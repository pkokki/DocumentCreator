using Azure.Storage.Blobs.Models;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator.Core.Azure
{
    public class BlobContentItem : ContentItem
    {
        public BlobContentItem(Uri baseUri, BlobItem item, Stream stream)
        {
            var blobFileName = item.Name;
            var blobVersion = item.Metadata[AzureBlobRepository.VERSION_KEY];
            var name = $"{System.IO.Path.GetFileNameWithoutExtension(blobFileName)}_{blobVersion}";
            Name = name;
            Version = blobVersion;
            FileName = blobFileName;
            Path = $"{baseUri}/{blobFileName}";
            Size = (int)item.Properties.ContentLength;
            Timestamp = item.Properties.LastModified.Value.LocalDateTime;
            Buffer = stream;
        }

        public BlobContentItem(Uri baseUri, string blobFileName, BlobDownloadInfo info)
        {
            var blobVersion = info.Details.Metadata[AzureBlobRepository.VERSION_KEY];
            var name = $"{System.IO.Path.GetFileNameWithoutExtension(blobFileName)}_{blobVersion}";
            Name = name;
            Version = blobVersion;
            FileName = blobFileName;
            Path = $"{baseUri}/{blobFileName}";
            Size = (int)info.ContentLength;
            Timestamp = info.Details.LastModified.LocalDateTime;
            Buffer = info.Content;
        }

        public string Version { get; set; }
    }
}
