﻿using System;
using System.IO;
using System.Linq;

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
            var templateFilename = Path.Combine(TemplatesFolder, $"{templateName}_{DateTime.Now.Ticks}.docx");
            File.WriteAllBytes(templateFilename, contents);
            return new ContentItem()
            {
                Name = templateName,
                FileName = Path.GetFileNameWithoutExtension(templateFilename),
                Buffer = contents
            };
        }

        public ContentItem CreateMapping(string templateName, string mappingName, byte[] contents)
        {
            var templateVersionName = GetLatestTemplateVersionName(templateName);
            var mappingFileName = Path.Combine(MappingsFolder, $"{templateVersionName}_{mappingName}_{DateTime.Now.Ticks}.xlsm");
            File.WriteAllBytes(mappingFileName, contents);
            return new ContentItem()
            {
                Name = Path.GetFileNameWithoutExtension(mappingFileName),
                FileName = Path.GetFileName(mappingFileName),
                Buffer = contents
            };
        }

        public ContentItem CreateDocument(string templateName, string mappingName, byte[] contents)
        {
            var templateVersionName = GetLatestTemplateVersionName(templateName);
            var mappingVersionName = GetLatestMappingVersionName($"{templateVersionName}_{mappingName}");
            var documentFileName = Path.Combine(DocumentsFolder, $"{mappingVersionName}_{DateTime.Now.Ticks}.docx");
            File.WriteAllBytes(documentFileName, contents);
            return new ContentItem()
            {
                Name = Path.GetFileNameWithoutExtension(documentFileName),
                FileName = Path.GetFileName(documentFileName),
                Buffer = contents
            };
        }

        public ContentItem GetLatestTemplate(string templateName)
        {
            var templateFileName = Directory
                .GetFiles(TemplatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            if (templateFileName == null)
                return null;
            return new ContentItem()
            {
                Name = Path.GetFileNameWithoutExtension(templateName),
                FileName = Path.GetFileName(templateFileName),
                Buffer = File.ReadAllBytes(templateFileName)
            };
        }

        public ContentItem GetLatestMapping(string templateName, string mappingName)
        {
            var templateVersionName = GetLatestTemplateVersionName(templateName);
            var mappingVersionName = GetLatestMappingVersionName($"{templateVersionName}_{mappingName}");
            if (mappingVersionName == null)
                return null;
            var mappingFileName = Path.Combine(MappingsFolder, $"{mappingVersionName}.xlsm");
            return new ContentItem()
            {
                Name = mappingVersionName,
                FileName = Path.GetFileName(mappingFileName),
                Buffer = File.ReadAllBytes(mappingFileName)
            };
        }

        public ContentItem GetEmptyMapping()
        {
            var emptyMappingPath = Path.Combine(rootPath, "resources", "empty_mappings.xlsm");
            return new ContentItem()
            {
                Name = null,
                FileName = null,
                Buffer = File.ReadAllBytes(emptyMappingPath)
            };
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
                    templatesFolder = Path.Combine(rootPath, "temp", "templates");
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
                    mappingsFolder = Path.Combine(rootPath, "temp", "mappings");
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
                    documentsFolder = Path.Combine(rootPath, "temp", "documents");
                    if (!Directory.Exists(documentsFolder))
                        Directory.CreateDirectory(documentsFolder);
                }
                return documentsFolder;
            }
        }
    }
}
