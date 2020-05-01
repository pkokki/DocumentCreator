using Azure;
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
    /// https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
    /// </summary>
    public class AzureBlobRepository : IRepository
    {
        // Retrieve the connection string for use with the application. The storage
        // connection string is stored in an environment variable on the machine
        // running the application called AZURE_STORAGE_CONNECTION_STRING. If the
        // environment variable is created after the application is launched in a
        // console or with Visual Studio, the shell or application needs to be closed
        // and reloaded to take the environment variable into account.
        private static readonly string ENV_CONN_STRING = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        private readonly BlobServiceClient blobServiceClient;
        private BlobContainerClient documentsContainer, templateContainer, mappingsContainer;

        public AzureBlobRepository() : this(ENV_CONN_STRING)
        {
        }
        public AzureBlobRepository(string connectionString) : this(new BlobServiceClient(connectionString))
        {
        }
        public AzureBlobRepository(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        private async Task<BlobContentInfo> UploadAsync(BlobClient blobClient, Stream contents)
        {
            contents.Position = 0;
            var response = await blobClient.UploadAsync(contents, false);
            return response.Value;
        }

        public async Task<ContentItem> CreateTemplate(string templateName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));

            var blobName = $"{templateName}_{DateTime.Now.Ticks}";
            var blobFileName = $"{blobName}.docx";
            var containerClient = await GetTemplatesContainerAsync();
            var blobClient = containerClient.GetBlobClient(blobFileName);
            var blobContentInfo = await UploadAsync(blobClient, contents);

            return new ContentItem()
            {
                Name = blobName,
                FileName = blobFileName,
                Path = blobClient.Uri.ToString(),
                Timestamp = blobContentInfo.LastModified.DateTime,
                Size = (int)contents.Length,
                Buffer = contents
            };
        }

        public IEnumerable<ContentItemSummary> GetTemplates()
        {
            throw new NotImplementedException();
            //var templates = Directory.GetFiles(TemplatesFolder, "*.docx")
            //    .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
            //    .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
            //    .GroupBy(a => a.Name)
            //    .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
            //    .Select(a => new FileContentItemSummary(a.Data.FullName));
            //return templates;
        }


        public ContentItem CreateMapping(string templateName, string mappingName, Stream contents)
        {
            throw new NotImplementedException();
        }

        public ContentItem CreateDocument(string templateName, string mappingName, Stream contents)
        {

            // Check
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            if (contents == null || contents.Length <= 0) throw new ArgumentNullException(nameof(contents));

            throw new NotImplementedException();

            // Prepare
            //var templateVersionName = GetLatestTemplateVersionName(templateName);
            //if (templateVersionName == null)
            //    throw new ArgumentException($"Template {templateName} not found.");
            //var mappingVersionName = GetLatestMappingVersionName($"{templateVersionName}_{mappingName}");
            //if (mappingVersionName == null)
            //    throw new ArgumentException($"Mapping {mappingName} not found.");

            //// Execute
            //var blobName = $"{mappingVersionName}_{DateTime.Now.Ticks}";
            //var blobFileName = $"{blobName}.docx";
            //var containerClient = await GetDocumentsContainerAsync();
            //var blobClient = containerClient.GetBlobClient(blobFileName);
            //var blobContentInfo = await UploadAsync(blobClient, contents);

            //// Respond
            //return new ContentItem()
            //{
            //    Name = blobName,
            //    FileName = blobName,
            //    Path = blobClient.Uri.ToString(),
            //    Timestamp = blobContentInfo.LastModified.DateTime,
            //    Size = contents.Length,
            //    Buffer = contents
            //};
        }


        private async Task<BlobContainerClient> GetDocumentsContainerAsync()
        {
            if (documentsContainer == null)
            {
                documentsContainer = blobServiceClient.GetBlobContainerClient("DOCUMENTS");
                await documentsContainer.CreateIfNotExistsAsync();
            }
            return documentsContainer;
        }
        private async Task<BlobContainerClient> GetTemplatesContainerAsync()
        {
            if (templateContainer == null)
            {
                templateContainer = blobServiceClient.GetBlobContainerClient("TEMPLATES");
                await templateContainer.CreateIfNotExistsAsync();
            }
            return templateContainer;
        }
        private async Task<BlobContainerClient> GetMappingsContainerAsync()
        {
            if (mappingsContainer == null)
            {
                mappingsContainer = blobServiceClient.GetBlobContainerClient("MAPPINGS");
                await mappingsContainer.CreateIfNotExistsAsync();
            }
            return mappingsContainer;
        }

        
        public ContentItem GetDocument(string documentId)
        {
            throw new NotImplementedException();
            //// Check
            //if (string.IsNullOrEmpty(documentId))
            //    throw new ArgumentNullException(nameof(documentId));

            //// Prepare
            //var pathPattern = $"*_{documentId}.docx";
            //var blobFileName = Directory
            //    .GetFiles(DocumentsFolder, pathPattern)
            //    .FirstOrDefault();
            //if (blobFileName == null)
            //    return null;
            //var blobName = Path.GetFileNameWithoutExtension(blobFileName);

            //// Execute
            //var containerClient = await GetDocumentsContainerAsync();
            //var blobClient = containerClient.GetBlobClient(blobFileName);
            //var response = await blobClient.DownloadAsync();
            //var blobDownloadInfo = response.Value;

            //// Respond
            //var contentStream = new MemoryStream();
            //blobDownloadInfo.Content.CopyTo(contentStream);
            //return new ContentItem()
            //{
            //    Name = blobName,
            //    FileName = blobFileName,
            //    Path = blobClient.Uri.ToString(),
            //    Timestamp = blobDownloadInfo.Details.LastModified.DateTime,
            //    Size = (int)blobDownloadInfo.ContentLength,
            //    Buffer = contentStream.ToArray()
            //};
        }

        

        public IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null)
        {
            throw new NotImplementedException();
        }

        public Stream GetEmptyMapping()
        {
            throw new NotImplementedException();
        }

        public ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName)
        {
            throw new NotImplementedException();
        }

        public ContentItem GetLatestTemplate(string templateName)
        {
            throw new NotImplementedException();
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

        public ContentItem GetTemplate(string templateName, string version = null)
        {
            throw new NotImplementedException();
        }

        
        public IEnumerable<ContentItemSummary> GetTemplateVersions(string templateName)
        {
            throw new NotImplementedException();
        }

        public void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images)
        {
            throw new NotImplementedException();
        }
    }
}
