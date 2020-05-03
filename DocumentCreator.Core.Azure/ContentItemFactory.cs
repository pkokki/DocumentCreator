using Azure.Storage.Blobs.Models;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator.Core.Azure
{
    internal static class ContentItemFactory
    {
        private const long DEFAULT_CONTENT_LENGTH = 0;

        internal static TemplateContentSummary BuildTemplateSummary(string blobUri, BlobItem blobItem)
        {
            return new TemplateContentSummary()
            {
                Name = Path.GetFileNameWithoutExtension(blobItem.Name),
                TemplateName = blobItem.Metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = blobItem.Metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                FileName = blobItem.Name,
                Path = blobUri,
                Timestamp = blobItem.Properties.LastModified.Value.LocalDateTime,
                Size = blobItem.Properties.ContentLength ?? DEFAULT_CONTENT_LENGTH,
            };
        }
        private static TemplateContent BuildTemplate(Uri blobUri, IDictionary<string, string> metadata, DateTime timeStamp, long? contentLength, Stream contents)
        {
            var blobPath = blobUri.ToString();
            var blobFileName = blobPath;
            return new TemplateContent()
            {
                Name = Path.GetFileNameWithoutExtension(blobFileName),
                TemplateName = metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                FileName = Path.GetFileName(blobFileName),
                Path = blobPath,
                Timestamp = timeStamp,
                Size = contentLength ?? DEFAULT_CONTENT_LENGTH,
                Buffer = contents
            };
        }
        internal static TemplateContent BuildTemplate(Uri blobUri, BlobItem blobItem, Stream contents)
        {
            return BuildTemplate(blobUri, blobItem.Metadata, blobItem.Properties.CreatedOn.Value.LocalDateTime, blobItem.Properties.ContentLength, contents);
        }
        internal static TemplateContent BuildTemplate(Uri blobUri, BlobDownloadInfo blobDownloadInfo)
        {
            return BuildTemplate(blobUri, blobDownloadInfo.Details.Metadata, blobDownloadInfo.Details.LastModified.LocalDateTime, blobDownloadInfo.ContentLength, blobDownloadInfo.Content);
        }
        internal static TemplateContent BuildTemplate(Uri blobUri, BlobContentInfo blobContentInfo, Dictionary<string, string> metadata, Stream contents)
        {
            return BuildTemplate(blobUri, metadata, blobContentInfo.LastModified.LocalDateTime, contents.Length, contents);
        }
        

        internal static MappingContentSummary BuildMappingSummary(string blobUri, BlobItem blobItem)
        {
            return new MappingContentSummary()
            {
                Name = Path.GetFileNameWithoutExtension(blobItem.Name),
                TemplateName = blobItem.Metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = blobItem.Metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                MappingName = blobItem.Metadata[AzureBlobRepository.MAPPING_NAME_KEY],
                MappingVersion = blobItem.Metadata[AzureBlobRepository.MAPPING_VERSION_KEY],
                FileName = blobItem.Name,
                Path = blobUri,
                Timestamp = blobItem.Properties.LastModified.Value.LocalDateTime,
                Size = blobItem.Properties.ContentLength ?? DEFAULT_CONTENT_LENGTH
            };
        }
        private static MappingContent BuildMapping(Uri blobUri, IDictionary<string, string> metadata, DateTime timeStamp, long? contentLength, Stream contents)
        {
            var blobPath = blobUri.ToString();
            var blobFileName = blobPath;
            return new MappingContent()
            {
                Name = Path.GetFileNameWithoutExtension(blobFileName),
                TemplateName = metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                MappingName = metadata[AzureBlobRepository.MAPPING_NAME_KEY],
                MappingVersion = metadata[AzureBlobRepository.MAPPING_VERSION_KEY],
                FileName = Path.GetFileName(blobFileName),
                Path = blobPath,
                Timestamp = timeStamp,
                Size = contentLength ?? DEFAULT_CONTENT_LENGTH,
                Buffer = contents
            };
        }
        internal static MappingContent BuildMapping(Uri blobUri, BlobItem blobItem, Stream contents)
        {
            return BuildMapping(blobUri, 
                blobItem.Metadata, 
                blobItem.Properties.CreatedOn.Value.LocalDateTime, 
                blobItem.Properties.ContentLength, 
                contents);
        }
        internal static MappingContent BuildMapping(Uri blobUri, BlobDownloadInfo blobDownloadInfo)
        {
            return BuildMapping(blobUri, 
                blobDownloadInfo.Details.Metadata, 
                blobDownloadInfo.Details.LastModified.LocalDateTime, 
                blobDownloadInfo.ContentLength, 
                blobDownloadInfo.Content);
        }
        internal static MappingContent BuildMapping(Uri blobUri, BlobContentInfo blobContentInfo, Dictionary<string, string> metadata, Stream contents)
        {
            return BuildMapping(blobUri,
                metadata, 
                blobContentInfo.LastModified.LocalDateTime,
                contents.Length,
                contents);
        }

        internal static DocumentContentSummary BuildDocumentSummary(Uri blobUri, BlobItem blobItem)
        {
            return new DocumentContentSummary()
            {
                Name = Path.GetFileNameWithoutExtension(blobItem.Name),
                TemplateName = blobItem.Metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = blobItem.Metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                MappingName = blobItem.Metadata[AzureBlobRepository.MAPPING_NAME_KEY],
                MappingVersion = blobItem.Metadata[AzureBlobRepository.MAPPING_VERSION_KEY],
                Identifier = blobItem.Metadata[AzureBlobRepository.DOCUMENT_ID],
                FileName = blobItem.Name,
                Path = blobUri.ToString(),
                Timestamp = blobItem.Properties.LastModified.Value.LocalDateTime,
                Size = blobItem.Properties.ContentLength ?? DEFAULT_CONTENT_LENGTH
            };
        }
        private static DocumentContent BuildDocument(Uri blobUri, IDictionary<string, string> metadata, DateTime timeStamp, Stream contents)
        {
            var blobPath = blobUri.ToString();
            var blobFileName = blobPath;
            return new DocumentContent()
            {
                Name = Path.GetFileNameWithoutExtension(blobFileName),
                TemplateName = metadata[AzureBlobRepository.TEMPLATE_NAME_KEY],
                TemplateVersion = metadata[AzureBlobRepository.TEMPLATE_VERSION_KEY],
                MappingName = metadata[AzureBlobRepository.MAPPING_NAME_KEY],
                MappingVersion = metadata[AzureBlobRepository.MAPPING_VERSION_KEY],
                Identifier = metadata[AzureBlobRepository.DOCUMENT_ID],
                FileName = Path.GetFileName(blobFileName),
                Path = blobPath,
                Timestamp = timeStamp,
                Size = contents.Length,
                Buffer = contents
            };
        }
        internal static DocumentContent BuildDocument(Uri blobUri, BlobContentInfo blobContentInfo, Dictionary<string, string> metadata, Stream contents)
        {
            return BuildDocument(blobUri, metadata, blobContentInfo.LastModified.LocalDateTime, contents);
        }

    }
}
