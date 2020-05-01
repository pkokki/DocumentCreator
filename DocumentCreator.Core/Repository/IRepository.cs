using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreator.Core.Repository
{
    public interface IRepository
    {
        IEnumerable<ContentItemSummary> GetTemplates();
        IEnumerable<ContentItemSummary> GetTemplateVersions(string templateName);


        Task<ContentItem> CreateTemplate(string templateName, Stream contents);
        ContentItem GetLatestTemplate(string templateName);
        ContentItem GetTemplate(string templateName, string version = null);


        IEnumerable<ContentItemSummary> GetMappings(string templateName, string templateVersion, string mappingName = null);
        IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null);
        Stream GetEmptyMapping();
        ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName);
        ContentItem GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion);
        ContentItem CreateMapping(string templateName, string mappingName, Stream contents);


        IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null);
        ContentItem CreateDocument(string templateName, string mappingName, Stream contents);
        ContentItem GetDocument(string documentId);
        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    }
}
