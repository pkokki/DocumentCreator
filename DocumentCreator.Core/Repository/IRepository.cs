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
        Task<ContentItem> CreateMapping(string templateName, string mappingName, Stream contents);
        //Stream GetEmptyMapping();
        ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName);
        Task<ContentItem> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion);


        IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null);
        Task<ContentItem> CreateDocument(string templateName, string mappingName, Stream contents);
        ContentItem GetDocument(string documentId);
        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    }
}
