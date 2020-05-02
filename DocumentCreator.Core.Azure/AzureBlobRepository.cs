using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal const string VERSION_KEY = "DC_VERSION";

        /// <summary>
        /// The <see cref="BlobServiceClient"/> used to access the containers.
        /// </summary>
        private readonly BlobServiceClient blobServiceClient;

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
        /// the connection string from the environment variable AZURE_STORAGE_CONNECTION_STRING (if exists).
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
        public AzureBlobRepository(string connectionString) : this(new BlobServiceClient(connectionString))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobRepository"/> class.
        /// </summary>
        /// <param name="blobServiceClient">
        /// A <see cref="BlobServiceClient"/> used to access the Azure Blob service.
        /// </param>
        public AzureBlobRepository(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        #endregion

        #region Template-related interface methods

        public IEnumerable<ContentItemSummary> GetTemplates()
        {
            var containerClient = GetTemplatesContainer();
            var blobs = containerClient.GetBlobs(BlobTraits.Metadata);
            return blobs
                .Select(o => new BlobContentItemSummary(containerClient.Uri, o))
                .ToList();
        }

        public IEnumerable<ContentItemSummary> GetTemplateVersions(string templateName)
        {
            var containerClient = GetTemplatesContainer();
            return containerClient.GetBlobs(BlobTraits.Metadata, BlobStates.Snapshots, templateName)
                .Select(o => new BlobContentItemSummary(containerClient.Uri, o))
                .ToList();
        }

        public ContentItem GetLatestTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            try
            {
                return DownloadContentItem(GetTemplatesContainer(), $"{templateName}.docx");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        public ContentItem GetTemplate(string templateName, string templateVersion = null)
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
                    .FirstOrDefault(o => o.Metadata[VERSION_KEY] == templateVersion);
                if (blobItem == null)
                    return null;
                return DownloadContentItem(GetTemplatesContainer(), blobItem);
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
        public async Task<ContentItem> CreateTemplate(string templateName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));

            var containerClient = GetTemplatesContainer();
            var blobFileName = $"{templateName}.docx";
            var blobClient = containerClient.GetBlobClient(blobFileName);

            var metadata = await CreateSnapshotAndVersionMetadata(blobClient);
            var version = metadata[VERSION_KEY];
            var response = await blobClient.UploadAsync(contents, metadata: metadata);
            var blobContentInfo = response.Value;

            return new ContentItem()
            {
                Name = $"{templateName}_{version}",
                FileName = blobFileName,
                Path = blobClient.Uri.ToString(),
                Timestamp = blobContentInfo.LastModified.LocalDateTime,
                Size = (int)contents.Length,
                Buffer = contents
            };

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
        public async Task<ContentItem> CreateMapping(string templateName, string mappingName, Stream contents)
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

            var metadata = await CreateSnapshotAndVersionMetadata(blobClient);
            var version = metadata[VERSION_KEY];
            var response = await blobClient.UploadAsync(contents, metadata: metadata);
            var blobContentInfo = response.Value;

            var name = $"{templateName}_{templateVersion}_{mappingName}_{version}";
            return new ContentItem()
            {
                Name = name,
                FileName = blobFileName,
                Path = blobClient.Uri.ToString(),
                Timestamp = blobContentInfo.LastModified.LocalDateTime,
                Size = (int)contents.Length,
                Buffer = contents
            };
        }

        public ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));

            if (string.IsNullOrEmpty(templateVersion))
                templateVersion = GetLatestTemplateVersion(templateName).GetAwaiter().GetResult();
            if (string.IsNullOrEmpty(templateVersion)) throw new ArgumentNullException(nameof(templateVersion));

            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            var mappingVersionName = $"{templateName}_{templateVersion}_{mappingName}";
            try
            {
                return DownloadContentItem(GetMappingsContainer(), $"{mappingVersionName}.xlsm");
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404)
                    throw ex;
            }
            return null;
        }

        public ContentItem GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItemSummary> GetMappings(string templateName, string templateVersion, string mappingName = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null)
        {
            throw new NotImplementedException();
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
        public async Task<ContentItem> CreateDocument(string templateName, string mappingName, Stream contents)
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

            var documentId = DateTime.Now.Ticks;
            var containerClient = GetDocumentsContainer();
            var name = $"{templateName}_{templateVersion}_{mappingName}_{mappingVersion}_{documentId}";
            var blobFileName = $"{name}.docx";
            var blobClient = containerClient.GetBlobClient(blobFileName);
            
            var response = await blobClient.UploadAsync(contents, metadata: null);
            var blobContentInfo = response.Value;
            
            return new ContentItem()
            {
                Name = name,
                FileName = blobFileName,
                Path = blobClient.Uri.ToString(),
                Timestamp = blobContentInfo.LastModified.LocalDateTime,
                Size = (int)contents.Length,
                Buffer = contents
            };
        }

        public ContentItem GetDocument(string documentId)
        {
            throw new NotImplementedException();
            //if (string.IsNullOrEmpty(documentId))
            //    throw new ArgumentNullException(nameof(documentId));
            //var pathPattern = $"*_{documentId}.docx";
            //var documentFileName = Directory
            //    .GetFiles(DocumentsFolder, pathPattern)
            //    .FirstOrDefault();
            //if (documentFileName == null)
            //    return null;
            //return FileContentItem.Create(documentFileName);
        }

        public IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null)
        {
            throw new NotImplementedException();
        }

        public void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images)
        {
            throw new NotImplementedException();
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

        private ContentItem DownloadContentItem(BlobContainerClient blobContainerClient, string blobFileName)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobFileName);

            var response = blobClient.Download();
            if (response == null)
                return null;
            var blobDownloadInfo = response.Value;
            return new BlobContentItem(blobContainerClient.Uri, blobFileName, blobDownloadInfo);
        }

        private ContentItem DownloadContentItem(BlobContainerClient blobContainerClient, BlobItem blobItem)
        {
            var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

            var stream = new MemoryStream();
            if (string.IsNullOrEmpty(blobItem.Snapshot))
                blobClient.DownloadTo(stream);
            else
                blobClient.WithSnapshot(blobItem.Snapshot).DownloadTo(stream);

            return new BlobContentItem(blobContainerClient.Uri, blobItem, stream);
        }

        private async Task<IDictionary<string, string>> CreateSnapshotAndVersionMetadata(BlobClient blobClient) 
        { 
            var version = 1;
            IDictionary<string, string> metadata = new Dictionary<string, string>();
            try
            {
                var propResponse = await blobClient.GetPropertiesAsync();
                if (propResponse != null)
                {
                    metadata = propResponse.Value.Metadata;
                    if (metadata.TryGetValue(VERSION_KEY, out string prevVersion))
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
            metadata[VERSION_KEY] = Convert.ToString(version);
            return metadata;
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
                    if (metadata.TryGetValue(VERSION_KEY, out string version))
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
                    if (metadata.TryGetValue(VERSION_KEY, out string version))
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
