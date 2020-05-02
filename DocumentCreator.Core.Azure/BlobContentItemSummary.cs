using Azure.Storage.Blobs.Models;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Azure
{
    public class BlobContentItemSummary : ContentItemSummary
    {
        public BlobContentItemSummary(Uri baseUri, BlobItem item)
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
        }

        public string Version { get; set; }
    }
}
