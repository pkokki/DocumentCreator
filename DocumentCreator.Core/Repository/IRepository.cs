using System.Collections.Generic;

namespace DocumentCreator.Core.Repository
{
    public interface IRepository
    {
        IEnumerable<ContentItemSummary> GetTemplates();
        IEnumerable<ContentItemSummary> GetTemplateVersions(string templateName);


        ContentItem CreateTemplate(string templateName, byte[] contents);
        ContentItem GetLatestTemplate(string templateName);
        ContentItem GetTemplate(string templateName, string version = null);


        IEnumerable<ContentItemSummary> GetMappings(string templateName, string templateVersion, string mappingName = null);
        IEnumerable<ContentItemStats> GetMappingStats(string mappingName = null);
        byte[] GetEmptyMapping();
        ContentItem GetLatestMapping(string templateName, string templateVersion, string mappingName);
        ContentItem GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion);
        ContentItem CreateMapping(string templateName, string mappingName, byte[] contents);


        IEnumerable<ContentItemSummary> GetDocuments(string templateName = null, string templateVersion = null, string mappingsName = null, string mappingsVersion = null);
        ContentItem CreateDocument(string templateName, string mappingName, byte[] contents);
        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    }
}
