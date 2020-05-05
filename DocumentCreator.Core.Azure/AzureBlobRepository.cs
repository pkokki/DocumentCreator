using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentCreator.Core.Azure
{
    /// <summary>
    /// An implementation of <see cref="IRepository"/> for the 
    /// <a href="https://azure.microsoft.com/en-us/services/storage/blobs/">Azure Blob Storage</a>
    /// using the Azure Storage Blobs 
    /// <a href="https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/storage/Azure.Storage.Blobs">client library</a>
    /// for .NET.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet"/> 
    public class AzureBlobRepository : IRepository
    {

        #region Fields

        /// <summary>
        /// The storage connection string (if exists) from the environment variable AZURE_STORAGE_CONNECTION_STRING.
        /// </summary>
        /// <remarks>
        /// Retrieve the connection string for use with the application. The storage
        /// connection string is stored in an environment variable on the machine
        /// running the application called AZURE_STORAGE_CONNECTION_STRING. If the
        /// environment variable is created after the application is launched in a
        /// console or with Visual Studio, the shell or application needs to be closed
        /// and reloaded to take the environment variable into account.
        /// </remarks>
        private static readonly string ENV_CONN_STRING = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        
        /// <summary>
        /// The key used to store the version number in the blob metadata
        /// </summary>
        internal const string TEMPLATE_NAME_KEY = "dc-template-name";
        internal const string TEMPLATE_VERSION_KEY = "dc-template-version";
        internal const string MAPPING_NAME_KEY = "dc-mapping-name";
        internal const string MAPPING_VERSION_KEY = "dc-mapping-version";
        internal const string DOCUMENT_ID = "dc-document-id";

        /// <summary>
        /// The <see cref="BlobServiceClient"/> used to access the containers.
        /// </summary>
        protected readonly BlobServiceClient blobServiceClient;

        /// <summary>
        /// A lazy-loaded <see cref="BlobContainerClient"/> used to access the document container
        /// </summary>
        private BlobContainerClient documentsContainer;

        /// <summary>
        /// A lazy-loaded <see cref="BlobContainerClient"/> used to access the template container.
        /// </summary>
        private BlobContainerClient templateContainer;

        /// <summary>
        /// A lazy-loaded <see cref="BlobContainerClient"/> used to access the mappings container.
        /// </summary>
        private BlobContainerClient mappingsContainer;

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobRepository"/> class with 
        /// the connection string from the environment variable AZURE_STORAGE_CONNECTION_STRING 
        /// and the static website endpoint from the environment variable AZURE_STORAGE_STATIC_WEBSITE.
        /// </summary>
        public AzureBlobRepository() : this(ENV_CONN_STRING)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobRepository"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// A connection string includes the authentication information required for your application 
        /// to access data in an Azure Storage account at runtime.
        /// 
        /// For more information, <see href="https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string"/>.
        /// </param>
        public AzureBlobRepository(string connectionString)
        {
            this.blobServiceClient = new BlobServiceClient(connectionString);
        }

        #endregion

        #region Template-related interface methods

        public IEnumerable<TemplateContentSummary> GetTemplates()
        {
            var containerClient = GetTemplatesContainer();
            var blobs = containerClient.GetBlobs(BlobTraits.Metadata);
            return blobs
                .Select(o => ContentItemFactory.BuildTemplateSummary($"{containerClient.Uri}/{o.Name}", o))
                .ToList();
        }

        public IEnumerable<TemplateContentSummary> GetTemplateVersions(string templateName)
        {
            var containerClient = GetTemplatesContainer();
            return containerClient.GetBlobs(BlobTraits.Metadata, BlobStates.Snapshots, templateName)
                .Select(o => ContentItemFactory.BuildTemplateSummary($"{containerClient.Uri}/{o.Name}", o))
                .ToList();
        }

        public TemplateContent GetLatestTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            try
            {
                var containerClient = GetTemplatesContainer();
                var blobFileName = $"{templateName}.docx";
                var blobClient = containerClient.GetBlobClient(blobFileName);
                var blobDownloadInfo = DownloadContentItem(blobClient);
                return ContentItemFactory.BuildTemplate(blobClient.Uri, blobDownloadInfo);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        public TemplateContent GetTemplate(string templateName, string templateVersion = null)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(templateVersion))
            {
                return GetLatestTemplate(templateName);
            }
            else
            {
                var containerClient = GetTemplatesContainer();
                var blobItem = containerClient
                    .GetBlobs(BlobTraits.Metadata, BlobStates.Snapshots, templateName)
                    .FirstOrDefault(o => o.Metadata[TEMPLATE_VERSION_KEY] == templateVersion);
                if (blobItem == null)
                    return null;
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var contents = DownloadContentItem(blobClient, blobItem);
                return ContentItemFactory.BuildTemplate(blobClient.Uri, blobItem, contents);
            }
        }

        /// <summary>
        /// The <see cref="CreateTemplate(string, Stream)"/> operation creates a new template.
        /// If the template with the same name already exists, the operation creates a new version.
        /// </summary>
        /// <param name="templateName">The name of template to create.</param>
        /// <param name="contents">The <see cref="Stream"/> that contains a valid Word document.</param>
        /// <returns>A <see cref="ContentItem"/> describing the state of the created template.</returns>
        /// <remarks>
        /// An <see cref="ArgumentNullException"/> will be thrown if templateName or contents are null or empty.
        /// A <see cref="RequestFailedException"/> will be thrown if a failure occurs.
        /// </remarks>
        public async Task<TemplateContent> CreateTemplate(string templateName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));

            var containerClient = GetTemplatesContainer();
            var blobFileName = $"{templateName}.docx";
            var blobClient = containerClient.GetBlobClient(blobFileName);

            var templateVersion = await TryCreateSnapshot(blobClient, TEMPLATE_VERSION_KEY);

            var metadata = new Dictionary<string, string>
            {
                [TEMPLATE_NAME_KEY] = templateName,
                [TEMPLATE_VERSION_KEY] = templateVersion
            };

            var response = await blobClient.UploadAsync(contents, metadata: metadata);
            var blobContentInfo = response.Value;

            return ContentItemFactory.BuildTemplate(blobClient.Uri, blobContentInfo, metadata, contents);
        }

        #endregion

        #region Mapping-related interface methods

        /// <summary>
        /// The <see cref="CreateMapping(string, string, Stream)"/> operation creates a new mapping.
        /// If the mapping with the same name already exists, the operation creates a new version.
        /// </summary>
        /// <param name="templateName">The name of the parent template.</param>
        /// <param name="mappingName">The name of mapping to create.</param>
        /// <param name="contents">The <see cref="Stream"/> that contains a valid Excel document.</param>
        /// <returns>A <see cref="ContentItem"/> describing the state of the created mapping.</returns>
        /// <remarks>
        /// An <see cref="ArgumentNullException"/> will be thrown if any of the parameters are null or empty.
        /// A <see cref="RequestFailedException"/> will be thrown if a failure occurs.
        /// </remarks>
        public async Task<MappingContent> CreateMapping(string templateName, string mappingName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));

            var templateVersion = await GetLatestTemplateVersion(templateName);
            if (templateVersion == null)
                throw new ArgumentException(nameof(templateName));

            var containerClient = GetMappingsContainer();
            var blobFileName = $"{templateName}_{templateVersion}_{mappingName}.xlsm";
            var blobClient = containerClient.GetBlobClient(blobFileName);

            var mappingVersion = await TryCreateSnapshot(blobClient, MAPPING_VERSION_KEY);

            var metadata = new Dictionary<string, string>
            {
                [TEMPLATE_NAME_KEY] = templateName,
                [TEMPLATE_VERSION_KEY] = templateVersion,
                [MAPPING_NAME_KEY] = mappingName,
                [MAPPING_VERSION_KEY] = mappingVersion
            };

            var response = await blobClient.UploadAsync(contents, metadata: metadata);
            var blobContentInfo = response.Value;

            return ContentItemFactory.BuildMapping(blobClient.Uri, blobContentInfo, metadata, contents);
        }

        public MappingContent GetLatestMapping(string templateName, string templateVersion, string mappingName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));

            if (string.IsNullOrEmpty(templateVersion))
                templateVersion = GetLatestTemplateVersion(templateName).GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(templateVersion)) throw new ArgumentNullException(nameof(templateVersion));

            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            var mappingVersionName = $"{templateName}_{templateVersion}_{mappingName}";
            try
            {
                var containerClient = GetMappingsContainer();
                var blobFileName = $"{mappingVersionName}.xlsm";
                var blobClient = containerClient.GetBlobClient(blobFileName);
                var blobDownloadInfo = DownloadContentItem(blobClient);
                return ContentItemFactory.BuildMapping(blobClient.Uri, blobDownloadInfo);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        public async Task<MappingContent> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));

            if (string.IsNullOrEmpty(mappingVersion))
            {
                return GetLatestMapping(templateName, templateVersion, mappingName);
            }
            else
            {
                if (string.IsNullOrEmpty(templateVersion))
                    templateVersion = await GetLatestTemplateVersion(templateName);
                var prefix = $"{templateName}_{templateVersion}_{mappingName}";
                var containerClient = GetMappingsContainer();
                var blobItem = containerClient
                    .GetBlobs(BlobTraits.Metadata, BlobStates.Snapshots, prefix)
                    .FirstOrDefault(o => o.Metadata[MAPPING_VERSION_KEY] == mappingVersion);
                if (blobItem == null)
                    return null;
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var contents = DownloadContentItem(blobClient, blobItem);
                return ContentItemFactory.BuildMapping(blobClient.Uri, blobItem, contents);
            }
        }

        public IEnumerable<MappingContentSummary> GetMappings(string templateName, string templateVersion, string mappingName = null)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            
            var prefix = templateName;
            if (!string.IsNullOrEmpty(templateVersion))
            {
                prefix += "_" + templateVersion;
                if (!string.IsNullOrEmpty(mappingName))
                    prefix += "_" + mappingName;
            }
            var containerClient = GetMappingsContainer();
            var blobs = containerClient.GetBlobs(BlobTraits.Metadata, BlobStates.None, prefix);
            var items = blobs
                .Select(o => ContentItemFactory.BuildMappingSummary($"{containerClient.Uri}/{o.Name}", o));
            if (string.IsNullOrEmpty(templateVersion) && !string.IsNullOrEmpty(mappingName))
            {
                items = items.Where(o => o.Name.EndsWith(mappingName));
            }
            return items;
        }

        public IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null)
        {
            var documentsContainerClient = GetDocumentsContainer();
            var allDocuments = documentsContainerClient
                .GetBlobs(BlobTraits.Metadata, BlobStates.None, null)
                .Select(o => ContentItemFactory.BuildDocumentSummary($"{documentsContainerClient.Uri}/{o.Name}", o));

            var mappingContainerClient = GetMappingsContainer();
            var allMappings = mappingContainerClient
                .GetBlobs(BlobTraits.Metadata, BlobStates.None, null)
                .Select(o => ContentItemFactory.BuildMappingSummary($"{mappingContainerClient.Uri}/{o.Name}", o));
            if (string.IsNullOrEmpty(mappingName))
            {
                return allMappings
                    .GroupBy(o => o.MappingName)
                    .Select(g => new ContentItemStats()
                    {
                        MappingName = g.Key,
                        TemplateName = null,
                        Templates = g.Select(o => o.TemplateName).Distinct().Count(),
                        Documents = allDocuments.Count(d => d.MappingName == g.Key)
                    });
            }
            else
            {
                return allMappings
                    .Where(o => string.Equals(mappingName, o.MappingName, StringComparison.CurrentCultureIgnoreCase))
                    .GroupBy(o => o.TemplateName)
                    .Select(g => new ContentItemStats()
                    {
                        MappingName = mappingName,
                        TemplateName = g.Key,
                        Templates = 1,
                        Documents = allDocuments.Count(d => d.MappingName == mappingName && d.TemplateName == g.Key)
                    });
            }
        }

        #endregion

        #region Document-related interface methods

        /// <summary>
        /// The <see cref="CreateDocument(string, string, Stream)"/> operation creates a new document.
        /// The new document gets a unique name.
        /// </summary>
        /// <param name="templateName">The name of the template used for this document.</param>
        /// <param name="mappingName">The name of the mapping used to transform input.</param>
        /// <param name="contents">The <see cref="Stream"/> that contains a valid Word document.</param>
        /// <returns>A <see cref="ContentItem"/> describing the state of the created document.</returns>
        /// <remarks>
        /// An <see cref="ArgumentNullException"/> will be thrown if any of the parameters are null or empty.
        /// A <see cref="RequestFailedException"/> will be thrown if a failure occurs.
        /// </remarks>
        public async Task<DocumentContent> CreateDocument(string templateName, string mappingName, Stream contents)
        {
            // Check
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            if (contents == null || contents.Length <= 0) throw new ArgumentNullException(nameof(contents));

            var templateVersion = await GetLatestTemplateVersion(templateName);
            if (templateVersion == null)
                throw new ArgumentException($"Template {templateName} not found.");
            var mappingVersion = await GetLatestMappingVersion($"{templateName}_{templateVersion}_{mappingName}");
            if (mappingVersion == null)
                throw new ArgumentException($"Mapping {mappingName} not found.");

            var documentId = DateTime.Now.Ticks.ToString();
            var containerClient = GetDocumentsContainer();
            //var name = $"{templateName}_{templateVersion}_{mappingName}_{mappingVersion}_{documentId}";
            var blobFileName = $"{documentId}.docx";
            var blobClient = containerClient.GetBlobClient(blobFileName);

            var metadata = new Dictionary<string, string>
            {
                [TEMPLATE_NAME_KEY] = templateName,
                [TEMPLATE_VERSION_KEY] = templateVersion,
                [MAPPING_NAME_KEY] = mappingName,
                [MAPPING_VERSION_KEY] = mappingVersion,
                [DOCUMENT_ID] = documentId
            };

            var response = await blobClient.UploadAsync(contents, metadata: metadata);
            var blobContentInfo = response.Value;

            return ContentItemFactory.BuildDocument(blobClient.Uri, blobContentInfo, metadata, contents);
        }

        public DocumentContent GetDocument(string documentId)
        {
            if (string.IsNullOrEmpty(documentId)) throw new ArgumentNullException(nameof(documentId));
            var containerClient = GetDocumentsContainer();

            var blobName = $"{documentId}.docx";
            var blobClient = containerClient.GetBlobClient(blobName);

            DocumentContent document = null;
            try
            {
                var blobDownloadInfo = DownloadContentItem(blobClient);
                document = ContentItemFactory.BuildDocument(blobClient.Uri, blobDownloadInfo);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return document;
        }

        public IEnumerable<DocumentContentSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null)
        {
            var containerClient = GetDocumentsContainer();
            var documents = containerClient
                .GetBlobs(BlobTraits.Metadata, BlobStates.None, null)
                .Select(o => ContentItemFactory.BuildDocumentSummary($"{containerClient.Uri}/{o.Name}", o));

            if (!string.IsNullOrEmpty(templateName))
                documents = documents.Where(o => o.TemplateName == templateName);
            if (!string.IsNullOrEmpty(templateVersion))
                documents = documents.Where(o => o.TemplateVersion == templateVersion);
            if (!string.IsNullOrEmpty(mappingsName))
                documents = documents.Where(o => o.MappingName == mappingsName);
            if (!string.IsNullOrEmpty(mappingsVersion))
                documents = documents.Where(o => o.MappingVersion == mappingsVersion);
            return documents;
        }

        #endregion

        #region Azure Blob client utility methods (private)

        private BlobContainerClient GetDocumentsContainer()
        {
            if (documentsContainer == null)
            {
                documentsContainer = blobServiceClient.GetBlobContainerClient("DOCUMENTS");
                documentsContainer.CreateIfNotExists();
            }
            return documentsContainer;
        }

        private BlobContainerClient GetTemplatesContainer()
        {
            if (templateContainer == null)
            {
                templateContainer = blobServiceClient.GetBlobContainerClient("TEMPLATES");
                templateContainer.CreateIfNotExists();
            }
            return templateContainer;
        }

        private BlobContainerClient GetMappingsContainer()
        {
            if (mappingsContainer == null)
            {
                mappingsContainer = blobServiceClient.GetBlobContainerClient("MAPPINGS");
                mappingsContainer.CreateIfNotExists();
            }
            return mappingsContainer;
        }

        private BlobDownloadInfo DownloadContentItem(BlobClient blobClient)
        {

            var response = blobClient.Download();
            if (response == null)
                return null;
            var blobDownloadInfo = response.Value;
            return blobDownloadInfo;
        }

        private Stream DownloadContentItem(BlobClient blobClient, BlobItem blobItem)
        {

            var stream = new MemoryStream();
            if (string.IsNullOrEmpty(blobItem.Snapshot))
                blobClient.DownloadTo(stream);
            else
                blobClient.WithSnapshot(blobItem.Snapshot).DownloadTo(stream);
            return stream;
        }

        private async Task<string> TryCreateSnapshot(BlobClient blobClient, string versionKey) 
        { 
            var version = 1;
            try
            {
                var propResponse = await blobClient.GetPropertiesAsync();
                if (propResponse != null)
                {
                    var metadata = propResponse.Value.Metadata;
                    if (metadata.TryGetValue(versionKey, out string prevVersion))
                        version = int.Parse(prevVersion) + 1;
                    else
                        throw new InvalidOperationException("version not found in blob metadata");
                    await blobClient.CreateSnapshotAsync(metadata);
                }
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return Convert.ToString(version);
        }

        private async Task<string> GetLatestTemplateVersion(string templateName)
        {
            var containerClient = GetTemplatesContainer();
            var blobFileName = $"{templateName}.docx";
            var blobClient = containerClient.GetBlobClient(blobFileName);
            try
            {
                var propResponse = await blobClient.GetPropertiesAsync();
                if (propResponse != null)
                {
                    var metadata = propResponse.Value.Metadata;
                    if (metadata.TryGetValue(TEMPLATE_VERSION_KEY, out string version))
                        return version;
                }
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        private async Task<string> GetLatestMappingVersion(string mappingName)
        {
            var containerClient = GetMappingsContainer();
            var blobFileName = $"{mappingName}.xlsm";
            var blobClient = containerClient.GetBlobClient(blobFileName);
            try
            {
                var propResponse = await blobClient.GetPropertiesAsync();
                if (propResponse != null)
                {
                    var metadata = propResponse.Value.Metadata;
                    if (metadata.TryGetValue(MAPPING_VERSION_KEY, out string version))
                        return version;
                }
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        #endregion

    }
}
