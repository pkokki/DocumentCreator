﻿using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocumentCreator.Repository
{
    public class FileRepository : IRepository
    {
        private readonly string rootPath;
        private string templatesFolder;
        private string mappingsFolder;
        private string documentsFolder;

        public FileRepository(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public ContentItem CreateTemplate(string templateName, byte[] contents)
        {
            var templateVersionName = $"{templateName}_{DateTime.Now.Ticks}";
            var templateFilename = Path.Combine(TemplatesFolder, $"{templateVersionName}.docx");
            return new FileContentItem(templateFilename, contents);
        }

        public void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images)
        {
            var baseFolder = Path.Combine(rootPath, "dcfs", "files", "html");
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);
            if (html != null)
            {
                File.WriteAllText(Path.Combine(baseFolder, $"{htmlName}.html"), html, Encoding.UTF8);
            }
            if (images != null && images.Any())
            {
                var imageFolder = Path.Combine(baseFolder, htmlName);
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);
                foreach (var kvp in images)
                    File.WriteAllBytes(Path.Combine(imageFolder, kvp.Key), kvp.Value);
            }
        }

        public ContentItem CreateMapping(string templateName, string mappingName, byte[] contents)
        {
            var templateVersionName = GetLatestTemplateVersionName(templateName);
            var mappingFileName = Path.Combine(MappingsFolder, $"{templateVersionName}_{mappingName}_{DateTime.Now.Ticks}.xlsm");
            return new FileContentItem(mappingFileName, contents);
        }

        public ContentItem CreateDocument(string templateName, string mappingName, byte[] contents)
        {
            var templateVersionName = GetLatestTemplateVersionName(templateName);
            var mappingVersionName = mappingName != null
                ? GetLatestMappingVersionName($"{templateVersionName}_{mappingName}")
                : $"{templateVersionName}__";
            var documentFileName = Path.Combine(DocumentsFolder, $"{mappingVersionName}_{DateTime.Now.Ticks}.docx");
            return new FileContentItem(documentFileName, contents);
        }

        public ContentItem GetLatestTemplate(string templateName)
        {
            var templateFileName = Directory
                .GetFiles(TemplatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            if (templateFileName == null)
                return null;
            return FileContentItem.Create(templateFileName);
        }

        public ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName)
        {
            var templateVersionName = templateVersion == null 
                ? GetLatestTemplateVersionName(templateName)
                : $"{templateName}_{templateVersion}";
            var mappingVersionName = GetLatestMappingVersionName($"{templateVersionName}_{mappingName}");
            return GetTemplateMapping(mappingVersionName);
        }
        public ContentItem GetTemplateMapping(string mappingVersionName)
        {
            if (mappingVersionName == null)
                return null;
            var mappingFileName = Path.Combine(MappingsFolder, $"{mappingVersionName}.xlsm");
            return FileContentItem.Create(mappingFileName);
        }

        public byte[] GetEmptyMapping()
        {
            var emptyMappingPath = Path.Combine(rootPath, "dcfs", "empty_mappings_prod.xlsm");
            if (!File.Exists(emptyMappingPath))
            {
                var masterMappingPath = Path.Combine(rootPath, "resources", "empty_mappings.xlsm");
                File.Copy(masterMappingPath, emptyMappingPath);
            }
            var contents = File.ReadAllBytes(emptyMappingPath);
            return contents;
        }

        private string GetLatestTemplateVersionName(string templateName)
        {
            return Directory
                .GetFiles(TemplatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .Select(o => Path.GetFileNameWithoutExtension(o))
                .FirstOrDefault();
        }

        private string GetLatestMappingVersionName(string mappingName)
        {
            return Directory
                .GetFiles(MappingsFolder, $"{mappingName}_*.xlsm")
                .OrderByDescending(o => o)
                .Select(o => Path.GetFileNameWithoutExtension(o))
                .FirstOrDefault();
        }


        private string TemplatesFolder
        {
            get
            {
                if (templatesFolder == null)
                {
                    templatesFolder = Path.Combine(rootPath, "dcfs", "files", "templates");
                    if (!Directory.Exists(templatesFolder))
                        Directory.CreateDirectory(templatesFolder);
                }
                return templatesFolder;
            }
        }
        private string MappingsFolder
        {
            get
            {
                if (mappingsFolder == null)
                {
                    mappingsFolder = Path.Combine(rootPath, "dcfs", "files", "mappings");
                    if (!Directory.Exists(mappingsFolder))
                        Directory.CreateDirectory(mappingsFolder);
                }
                return mappingsFolder;
            }
        }
        private string DocumentsFolder
        {
            get
            {
                if (documentsFolder == null)
                {
                    documentsFolder = Path.Combine(rootPath, "dcfs", "files", "documents");
                    if (!Directory.Exists(documentsFolder))
                        Directory.CreateDirectory(documentsFolder);
                }
                return documentsFolder;
            }
        }

        public IEnumerable<ContentItemSummary> GetTemplates()
        {
            var templates = Directory.GetFiles(TemplatesFolder, "*.docx")
                .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
                .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
                .GroupBy(a => a.Name)
                .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
                .Select(a => new FileContentItemSummary(a.Data.FullName));
            return templates;
        }

        public ContentItem GetTemplate(string templateName, string version = null)
        {
            return string.IsNullOrEmpty(version)
                ? GetLatestTemplate(templateName)
                : FileContentItem.Create(TemplatesFolder, $"{templateName}_{version}.docx");
        }

        public IEnumerable<ContentItemSummary> GetTemplateVersions(string templateName)
        {
            var templates = Directory.GetFiles(TemplatesFolder, "*.docx")
                .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
                .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
                .Where(a => a.Name.Equals(templateName, StringComparison.CurrentCultureIgnoreCase))
                .Select(a => new FileContentItemSummary(a.FullName));
            return templates;
        }

        public IEnumerable<ContentItemSummary> GetMappings(string templateName, string templateVersion, string mappingName = null)
        {
            var mappings = Directory.GetFiles(MappingsFolder, $"{templateName ?? "*"}_{templateVersion ?? "*"}_{mappingName ?? "*"}_*.xlsm")
                .Select(f => new { FullName = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 4) })
                .Select(a => new { a.FullName, TemplateName = a.NameParts[0], TemplateVersion = a.NameParts[1], Name = a.NameParts[2], Version = a.NameParts[3] });
            if (mappingName == null)
                return mappings.GroupBy(a => a.Name)
                    .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
                    .OrderBy(a => a.Name).ThenBy(a => a.Data.Version)
                    .ToList()
                    .Select(a => new FileContentItemSummary(a.Data.FullName));
            else
                return mappings
                    .OrderBy(a => a.Version)
                    .ToList()
                    .Select(a => new FileContentItemSummary(a.FullName));
        }

        public ContentItem GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion)
        {
            var fullName = Directory.GetFiles(MappingsFolder, $"{templateName}_{templateVersion ?? "*"}_{mappingName}_{mappingVersion ?? "*"}.xlsm")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            if (fullName == null)
                return null;
            return FileContentItem.Create(fullName);
        }

        public IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null)
        {
            var mappings = Directory.GetFiles(MappingsFolder, "*.xlsm")
                .Select(f => new { FullName = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 4) })
                .Select(a => new { a.FullName, TemplateName = a.NameParts[0], MappingName = a.NameParts[2], Version = a.NameParts[3] })
                .Where(a => mappingName == null || string.Equals(a.MappingName, mappingName, StringComparison.InvariantCultureIgnoreCase))
                .GroupBy(a => mappingName == null ? a.MappingName : a.TemplateName)
                .Select(a => a.OrderBy(o => o.Version).First())
                .Select(a => new ContentItemStats()
                {
                    MappingName = a.MappingName,
                    TemplateName = a.TemplateName,
                    TimeStamp = new FileInfo(a.FullName).CreationTime,
                    Templates = mappingName != null ? 1 : Directory.GetFiles(MappingsFolder, $"*_*_{a.MappingName}_*.xlsm")
                        .Select(o => Path.GetFileNameWithoutExtension(o).Split('_', 2).First())
                        .Distinct()
                        .Count(),
                    Documents = Directory.GetFiles(DocumentsFolder, $"{(mappingName == null ? "*" : a.TemplateName)}_*_{a.MappingName}_*_*.docx")
                        .Length
                })
                .ToList();
            return mappings;
        }

        public IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null)
        {
            var pathPattern = $"{templateName ?? "*"}_{templateVersion ?? "*"}_{mappingsName ?? "*"}_{mappingsVersion ?? "*"}_*.docx";
            return Directory.GetFiles(DocumentsFolder, pathPattern).Select(path => new FileContentItemSummary(path));
        }
    }
}
