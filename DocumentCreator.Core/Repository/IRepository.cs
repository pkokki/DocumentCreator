using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreator.Core.Repository
{
    public interface IRepository
    {
        Task<TemplateContent> CreateTemplate(string templateName, Stream contents);
        IEnumerable<TemplateContentSummary> GetTemplates();
        TemplateContent GetTemplate(string templateName, string version = null);
        TemplateContent GetLatestTemplate(string templateName);
        IEnumerable<TemplateContentSummary> GetTemplateVersions(string templateName);


        Task<MappingContent> CreateMapping(string templateName, string mappingName, Stream contents);
        IEnumerable<MappingContentSummary> GetMappings(string templateName, string templateVersion, string mappingName = null);
        Task<MappingContent> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion);
        MappingContent GetLatestMapping(string templateName, string templateVersion, string mappingName);
        IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null);


        Task<DocumentContent> CreateDocument(string templateName, string mappingName, Stream contents);
        IEnumerable<DocumentContentSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null);
        DocumentContent GetDocument(string documentId);
        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    }
}
