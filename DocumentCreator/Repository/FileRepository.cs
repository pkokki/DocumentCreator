using DocumentCreator.Model;
using System;
using System.Collections.Generic;
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
                FullName = templateFilename,
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
                FullName = mappingFileName,
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
                FullName = documentFileName,
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
                Name = Path.GetFileNameWithoutExtension(templateFileName),
                FullName = templateFileName,
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
                FullName = mappingFileName,
                FileName = Path.GetFileName(mappingFileName),
                Buffer = File.ReadAllBytes(mappingFileName)
            };
        }

        public ContentItem GetEmptyMapping()
        {
            var emptyMappingPath = Path.Combine(rootPath, "temp", "empty_mappings_prod.xlsm");
            if (!File.Exists(emptyMappingPath))
            {
                var masterMappingPath = Path.Combine(rootPath, "resources", "empty_mappings.xlsm");
                File.Copy(masterMappingPath, emptyMappingPath);
            }
            return new ContentItem()
            {
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

        public IEnumerable<Template> GetTemplates()
        {
            var templates = Directory.GetFiles(TemplatesFolder)
                .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
                .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
                .GroupBy(a => a.Name)
                .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
                .Select(a => new { a.Name, a.Data.Version, Info = new FileInfo(a.Data.FullName) })
                .Select(a => new Template()
                {
                    Name = a.Name,
                    Version = a.Version,
                    Timestamp = a.Info.CreationTime,
                    Size = a.Info.Length,
                });
            return templates;
        }

        public Template GetTemplate(string templateName)
        {
            var latest = GetLatestTemplate(templateName);
            if (latest == null)
                return null;
            var info = new FileInfo(latest.FullName);
            return new Template()
            {
                Name = templateName,
                Version = latest.Name.Substring(templateName.Length + 1),
                Timestamp = info.CreationTime,
                Size = info.Length,
                Buffer = latest.Buffer
            };
        }
    }
}
