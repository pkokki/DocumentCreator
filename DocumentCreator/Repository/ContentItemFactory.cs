using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocumentCreator.Repository
{
    internal static class ContentItemFactory
    {
        internal static TemplateContentSummary BuildTemplateSummary(string fullName)
        {
            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            var info = new FileInfo(fullName);
            return new TemplateContentSummary()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                Timestamp = info.CreationTime
            };
        }
        internal static TemplateContent BuildTemplate(string fullName, Stream contents)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            var info = new FileInfo(fullName);
            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            return new TemplateContent()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                Timestamp = info.CreationTime,
                Buffer = contents
            };
        }
        internal static TemplateContent BuildTemplate(string folderName, string fileName, Stream contents)
        {
            return BuildTemplate(Path.Combine(folderName, fileName), contents);
        }

        internal static MappingContentSummary BuildMappingSummary(string fullName)
        {
            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            var info = new FileInfo(fullName);
            return new MappingContentSummary()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                MappingName = parts[2],
                MappingVersion = parts[3],
                Timestamp = info.CreationTime,
            };
        }
        internal static MappingContent BuildMapping(string fullName, Stream contents)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            var info = new FileInfo(fullName);
            return new MappingContent()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                MappingName = parts[2],
                MappingVersion = parts[3],
                Timestamp = info.CreationTime,
                Buffer = contents
            };
        }
        internal static MappingContent BuildMapping(string folderName, string fileName, Stream contents)
        {
            return BuildMapping(Path.Combine(folderName, fileName), contents);
        }

        internal static DocumentContentSummary BuildDocumentSummary(string fullName)
        {
            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            var info = new FileInfo(fullName);
            return new DocumentContentSummary()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                MappingName = parts[2],
                MappingVersion = parts[3],
                Identifier = parts[4],
                Timestamp = info.CreationTime,
            };
        }
        internal static DocumentContent BuildDocument(string fullName, Stream contents = null)
        {
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));

            var name = Path.GetFileNameWithoutExtension(fullName);
            var parts = name.Split('_');
            var info = new FileInfo(fullName);
            return new DocumentContent()
            {
                Name = name,
                Path = fullName,
                FileName = info.Name,
                Size = info.Length,
                TemplateName = parts[0],
                TemplateVersion = parts[1],
                MappingName = parts[2],
                MappingVersion = parts[3],
                Identifier = parts[4],
                Timestamp = info.CreationTime,
                Buffer = contents
            };
        }
        internal static DocumentContent BuildDocument(string folderName, string fileName, Stream contents = null)
        {
            return BuildDocument(Path.Combine(folderName, fileName), contents);
        }
    }
}
