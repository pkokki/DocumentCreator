using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Stream ReadContents(string folder, string fileName)
        {
            return ReadContents(Path.Combine(folder, fileName));
        }
        private Stream ReadContents(string path)
        {
            var contents = new MemoryStream();
            using (FileStream input = File.OpenRead(path))
            {
                input.CopyTo(contents);
            }
            if (contents.Position != 0)
                contents.Position = 0;
            return contents;
        }

        private void WriteContents(string folder, string fileName, Stream contents)
        {
            WriteContents(Path.Combine(folder, fileName), contents);
        }
        private void WriteContents(string path, Stream contents)
        {
            if (contents.Position != 0)
                contents.Position = 0;
            using FileStream output = File.Open(path, FileMode.Create);
            contents.CopyTo(output);
        }

        public Task<TemplateContent> CreateTemplate(string templateName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));
            
            var templateVersionName = $"{templateName}_{DateTime.Now.Ticks}";
            var templateFilename = $"{templateVersionName}.docx";
            var templatePath = Path.Combine(TemplatesFolder, templateFilename);
            WriteContents(templatePath, contents);
            TemplateContent result = ContentItemFactory.BuildTemplate(templatePath, contents);
            return Task.FromResult(result);
        }

        public Task<MappingContent> CreateMapping(string templateName, string mappingName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            if (contents == null || contents.Length == 0) throw new ArgumentNullException(nameof(contents));

            var templateVersion = GetLatestTemplateVersion(templateName);
            if (templateVersion == null)
                throw new ArgumentException($"Template {templateName} not found.");
            var mappingFileName = $"{templateName}_{templateVersion}_{mappingName}_{DateTime.Now.Ticks}.xlsm";
            WriteContents(MappingsFolder, mappingFileName, contents);
            var result = ContentItemFactory.BuildMapping(MappingsFolder, mappingFileName, contents);
            return Task.FromResult(result);
        }

        public Task<DocumentContent> CreateDocument(string templateName, string mappingName, Stream contents)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));
            if (contents == null || contents.Length <= 0) throw new ArgumentNullException(nameof(contents));

            var templateVersion = GetLatestTemplateVersion(templateName);
            if (templateVersion == null)
                throw new ArgumentException($"Template {templateName} not found.");
            var mappingVersion = GetLatestMappingVersion(templateName, templateVersion, mappingName);
            if (mappingVersion == null)
                throw new ArgumentException($"Mapping {mappingName} not found.");

            var documentFileName = Path.Combine(DocumentsFolder, $"{templateName}_{templateVersion}_{mappingName}_{mappingVersion}_{DateTime.Now.Ticks}.docx");
            WriteContents(documentFileName, contents);
            var result = ContentItemFactory.BuildDocument(documentFileName, contents);
            return Task.FromResult(result);
        }

        public TemplateContent GetLatestTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));

            var templateFileName = Directory
                .GetFiles(TemplatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            if (templateFileName == null)
                return null;
            return ContentItemFactory.BuildTemplate(templateFileName, ReadContents(templateFileName));
        }

        public MappingContent GetLatestMapping(string templateName, string templateVersion, string mappingName)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));

            if (string.IsNullOrEmpty(templateVersion))
                templateVersion = GetLatestTemplateVersion(templateName);
            if (string.IsNullOrEmpty(templateVersion)) throw new ArgumentNullException(nameof(templateName));

            var mappingVersion = GetLatestMappingVersion(templateName, templateVersion, mappingName);
            if (mappingVersion == null)
                return null;

            var mappingFileName = $"{templateName}_{templateVersion}_{mappingName}_{mappingVersion}.xlsm";
            var contents = ReadContents(MappingsFolder, mappingFileName);
            return ContentItemFactory.BuildMapping(MappingsFolder, mappingFileName, contents);
        }

        private string GetLatestTemplateVersion(string templateName)
        {
            return Directory
                .GetFiles(TemplatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .Select(o => Path.GetFileNameWithoutExtension(o).Split('_')[1] )
                .FirstOrDefault();
        }

        private string GetLatestMappingVersion(string templateName, string templateVersion, string mappingName)
        {
            return Directory
                .GetFiles(MappingsFolder, $"{templateName}_{templateVersion}_{mappingName}_*.xlsm")
                .OrderByDescending(o => o)
                .Select(o => Path.GetFileNameWithoutExtension(o).Split('_')[3])
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

        public IEnumerable<TemplateContentSummary> GetTemplates()
        {
            var templates = Directory.GetFiles(TemplatesFolder, "*.docx")
                .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
                .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
                .GroupBy(a => a.Name)
                .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
                .Select(a => ContentItemFactory.BuildTemplateSummary(a.Data.FullName));
            return templates;
        }

        public TemplateContent GetTemplate(string templateName, string version = null)
        {
            if (string.IsNullOrEmpty(version))
            {
                return GetLatestTemplate(templateName);
            }
            else
            {
                var templateFileName = $"{templateName}_{version}.docx";
                var templatePath = Path.Combine(TemplatesFolder, templateFileName);
                if (!File.Exists(templatePath))
                    return null;
                return ContentItemFactory.BuildTemplate(templatePath, ReadContents(templatePath));
            }
        }

        public IEnumerable<TemplateContentSummary> GetTemplateVersions(string templateName)
        {
            var templates = Directory.GetFiles(TemplatesFolder, "*.docx")
                .Select(f => new { Path = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 2) })
                .Select(a => new { FullName = a.Path, Name = a.NameParts[0], Version = a.NameParts[1] })
                .Where(a => a.Name.Equals(templateName, StringComparison.CurrentCultureIgnoreCase))
                .Select(a => ContentItemFactory.BuildTemplate(a.FullName, ReadContents(a.FullName)));
            return templates;
        }

        public IEnumerable<MappingContentSummary> GetMappings(string templateName, string templateVersion, string mappingName = null)
        {
            var mappings = Directory.GetFiles(MappingsFolder, $"{templateName ?? "*"}_{templateVersion ?? "*"}_{mappingName ?? "*"}_*.xlsm")
                .Select(f => new { FullName = f, NameParts = Path.GetFileNameWithoutExtension(f).Split('_', 4) })
                .Select(a => new { a.FullName, TemplateName = a.NameParts[0], TemplateVersion = a.NameParts[1], Name = a.NameParts[2], Version = a.NameParts[3] });
            if (mappingName == null)
                return mappings.GroupBy(a => a.Name)
                    .Select(ag => new { Name = ag.Key, Data = ag.OrderByDescending(o => o.Version).First() })
                    .OrderBy(a => a.Name).ThenBy(a => a.Data.Version)
                    .ToList()
                    .Select(a => ContentItemFactory.BuildMapping(a.Data.FullName, ReadContents(a.Data.FullName)));
            else
                return mappings
                    .OrderBy(a => a.Version)
                    .ToList()
                    .Select(a => ContentItemFactory.BuildMapping(a.FullName, ReadContents(a.FullName)));
        }

        public Task<MappingContent> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrEmpty(mappingName)) throw new ArgumentNullException(nameof(mappingName));

            var fullName = Directory.GetFiles(MappingsFolder, $"{templateName}_{templateVersion ?? "*"}_{mappingName}_{mappingVersion ?? "*"}.xlsm")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            MappingContent result = null;
            if (fullName != null && File.Exists(fullName))
                result = ContentItemFactory.BuildMapping(fullName, ReadContents(fullName));
            return Task.FromResult(result);
        }

        public IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null)
        {
            var searchPattern = mappingName == null ? "*.docx" : $"*_*_{mappingName}_*.docx";

            var allDocuments = Directory.GetFiles(DocumentsFolder, searchPattern)
                .Select(o => ContentItemFactory.BuildDocumentSummary(o));

            var allMappings = Directory.GetFiles(MappingsFolder, "*.xlsm")
                .Select(o => ContentItemFactory.BuildMappingSummary(o))
                .GroupBy(o => new { o.TemplateName, o.TemplateVersion, o.MappingName })
                .Select(o => o.OrderByDescending(o => o.TemplateVersion).First());

            if (string.IsNullOrEmpty(mappingName))
            {
                return allMappings
                    .GroupBy(o => o.MappingName)
                    .Select(g => new ContentItemStats()
                    {
                        MappingName = g.Key,
                        TemplateName = null,
                        Templates = g.Select(o => o.TemplateName).Distinct().Count(),
                        Documents = allDocuments.Count(d => d.MappingName == g.Key)
                    });
            }
            else
            {
                return allMappings
                    .Where(o => string.Equals(mappingName, o.MappingName, StringComparison.CurrentCultureIgnoreCase))
                    .GroupBy(o => o.TemplateName)
                    .Select(g => new ContentItemStats()
                    {
                        MappingName = mappingName,
                        TemplateName = g.Key,
                        Templates = 1,
                        Documents = allDocuments.Count(d => d.MappingName == mappingName && d.TemplateName == g.Key)
                    });
            }
        }

        public IEnumerable<DocumentContentSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingName = null, string mappingVersion = null)
        {
            var pathPattern = $"{templateName ?? "*"}_{templateVersion ?? "*"}_{mappingName ?? "*"}_{mappingVersion ?? "*"}_*.docx";
            return Directory.GetFiles(DocumentsFolder, pathPattern).Select(path => ContentItemFactory.BuildDocumentSummary(path));
        }

        public DocumentContent GetDocument(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException(nameof(documentId));
            var pathPattern = $"*_{documentId}.docx";
            var documentFileName = Directory
                .GetFiles(DocumentsFolder, pathPattern)
                .FirstOrDefault();
            if (documentFileName == null)
                return null;

            return ContentItemFactory.BuildDocument(documentFileName, ReadContents(documentFileName));
        }
    }
}
