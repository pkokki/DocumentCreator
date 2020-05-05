using Azure.Storage.Blobs;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentCreator.Core.Azure
{
    public class AzureBlobStaticHtmlRepository : IHtmlRepository
    {
        /// <summary>
        /// The storage connection string (if exists) from the environment variable AZURE_STORAGE_CONNECTION_STRING.
        /// </summary>
        private static readonly string ENV_CONN_STRING = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

        /// <summary>
        /// The primary endpoint of a static website of an Azure Storage Account from the environment variable AZURE_STORAGE_STATIC_WEBSITE.
        /// </summary>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website"/>
        private static readonly string ENV_STATIC_WEBSITE = Environment.GetEnvironmentVariable("AZURE_STORAGE_STATIC_WEBSITE");

        /// <summary>
        /// The <see cref="BlobServiceClient"/> used to access the containers.
        /// </summary>
        private readonly BlobServiceClient blobServiceClient;

        /// <summary>
        /// 
        /// </summary>
        private readonly string baseUrl;

        public AzureBlobStaticHtmlRepository() : this(ENV_CONN_STRING, ENV_STATIC_WEBSITE)
        {
        }

        public AzureBlobStaticHtmlRepository(string connectionString, string baseUrl)
            : this(new BlobServiceClient(connectionString), baseUrl)
        {
        }

        public AzureBlobStaticHtmlRepository(BlobServiceClient blobServiceClient, string baseUrl)
        {
            this.blobServiceClient = blobServiceClient;
            this.baseUrl = baseUrl;
        }

        public string GetUrl(string htmlName)
        {
            return $"{baseUrl}/{htmlName}.html";
        }


        public void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images)
        {
            var wwwContainerClient = blobServiceClient.GetBlobContainerClient("$web");
            wwwContainerClient.CreateIfNotExists();
            if (html != null)
            {
                var blobName = $"{htmlName}.html";
                var blobClient = wwwContainerClient.GetBlobClient(blobName);

                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream, Encoding.UTF8);
                writer.Write(html.Replace(htmlName + "/", htmlName + "_"));
                writer.Flush();
                stream.Position = 0;

                blobClient.Upload(stream);
            }
            if (images != null && images.Any())
            {
                foreach (var kvp in images)
                {
                    var blobName = $"{htmlName}_{kvp.Key}";
                    var blobClient = wwwContainerClient.GetBlobClient(blobName);
                    blobClient.Upload(new MemoryStream(kvp.Value));
                }
            }
        }
    }
}
