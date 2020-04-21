using DocumentCreator.Model;
using System.Collections.Generic;

namespace DocumentCreator.Repository
{
    public interface IRepository
    {
        ContentItem CreateTemplate(string templateName, byte[] contents);
        ContentItem GetLatestTemplate(string templateName);


        ContentItem GetEmptyMapping();
        ContentItem GetLatestMapping(string templateName, string mappingName);
        ContentItem CreateMapping(string templateName, string mappingName, byte[] contents);


        ContentItem CreateDocument(string templateName, string mappingName, byte[] contents);

        IEnumerable<Template> GetTemplates();
        Template GetTemplate(string templateName, string version = null);
        IEnumerable<Template> GetTemplateVersions(string templateName);
        IEnumerable<TemplateMapping> GetTemplateMappings(string templateName);
        TemplateMapping GetTemplateMapping(string templateName, string mappingName, string mappingVersion);

        void SaveHtml(string htmlName, string html, IDictionary<string, byte[]> images);
    
        IEnumerable<Mapping> GetMappings(string templateName = null);
        PagedResults<Document> GetDocuments(DocumentParams query);
    }
}
