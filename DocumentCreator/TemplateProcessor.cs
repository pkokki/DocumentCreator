using DocumentCreator.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public class TemplateProcessor
    {
        private readonly List<Template> templates = new List<Template>();


        // GET api/Templates
        public IEnumerable<Template> GetTemplates()
        {
            return templates;
        }
        // GET api/Mappings/{name}/Templates   <--------------------------- APP DEVELOPER
        public IEnumerable<Template> GetTemplatesForMapping(string name)
        {
            return templates.Where(o => o.TemplateMappings.Any(m => m.Name == name));
        }
        // POST api/Templates
        public Template CreateTemplate(string name)
        {
            var template = new Template(templates.LastOrDefault()?.Id + 1 ?? 1, name);
            templates.Add(template);
            return template;
        }
        // GET api/Templates/{templateId}
        public Template GetTemplate(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            return template;
        }
        // PUT api/Templates/{templateId}
        public Template UpdateTemplate(long templateId, string name)
        {
            var template = templates.First(o => o.Id == templateId);
            template.Name = name;
            return template;
        }
        // DELETE api/Templates/{templateId}
        public void DeactivateTemplate(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            templates.Remove(template);
        }
        



        // GET api/Templates/{templateId}/Versions
        public IEnumerable<TemplateVersion> GetTemplateVersions(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            return template.Versions;
        }
        // POST api/Templates/{templateId}/Versions
        public TemplateVersion CreateTemplateVersion(long templateId, byte[] buffer)
        {
            var template = templates.First(o => o.Id == templateId);
            var version = template.AddVersion(buffer);
            return version;
        }
        // GET api/Templates/{templateId}/Versions/{versionId}
        public TemplateVersion GetTemplateVersion(long templateId, long versionId)
        {
            return templates.First(o => o.Id == templateId).Versions.First(o => o.Id == versionId);
        }
        // PUT api/Templates/{templateId}/Versions/{versionId}
        // DELETE api/Templates/{templateId}/Versions/{versionId}




        // GET api/Templates/{templateId}/Fields
        public IEnumerable<TemplateField> GetTemplateFields(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            return FindTemplateFields(template.Versions.Last().Buffer);
        }
        // GET api/Templates/{templateId}/Fields/{versionId}
        public IEnumerable<TemplateField> GetTemplateFields(long templateId, long versionId)
        {
            return FindTemplateFields(GetTemplateVersion(templateId, versionId).Buffer);
        }
        // POST api/TemplateFields <-------------- SPECIAL HELPER
        public IEnumerable<TemplateField> FindTemplateFields(byte[] buffer)
        {
            using var ms = new MemoryStream(buffer);
            using var doc = WordprocessingDocument.Open(ms, false);
            
            var x = doc.MainDocumentPart.Document.Body.Descendants<SdtElement>();
            var y = x.Select(o => o.GetType().Name);
            
            return doc.MainDocumentPart.Document.Body
                .Descendants<SdtElement>()
                .Select(e => new TemplateField(e))
                .ToList();
        }



        // GET api/Templates/{templateId}/Mappings
        public IEnumerable<TemplateMapping> GetMappings(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            return template.TemplateMappings;
        }
        // POST api/Templates/{templateId}/Mappings/{name}
        public TemplateMapping UpsertMapping(long templateId, string mappingName, JObject transformations)
        {
            var template = templates.First(o => o.Id == templateId);
            return template.UpsertMapping(mappingName, transformations);
        }
        // GET api/Templates/{templateId}/Mappings/{name}
        public TemplateMapping GetMapping(long templateId, string name)
        {
            var template = templates.First(o => o.Id == templateId);
            return template.TemplateMappings.FirstOrDefault(o => o.Name == name);
        }



        // GET api/Documents/{documentId}   <--------------------------- APP DEVELOPER
        public TemplateDocument GetDocument(long documentId)
        {
            return templates.SelectMany(o => o.Documents).FirstOrDefault(d => d.Id == documentId);
        }
        // GET api/Templates/{templateId}/Documents
        public IEnumerable<TemplateDocument> GetDocuments(long templateId)
        {
            var template = templates.First(o => o.Id == templateId);
            return template.Documents;
        }
        // POST api/Documents/{templateId}
        public TemplateDocument CreateDocument(long templateId, JObject payload)
        {
            return CreateDocument(templateId, payload, sv => sv);
        }
        // POST api/Documents/{templateId}/{name}   <--------------------------- APP DEVELOPER
        public TemplateDocument CreateDocument(long templateId, string mappingName, JObject payload)
        {
            var template = templates.First(o => o.Id == templateId);
            var mapping = template.TemplateMappings.First(o => o.Name == mappingName);
            return CreateDocument(templateId, payload, 
                sv => mapping.Transformations.First(t => t.Key == sv).Value.ToString());
        }
        private TemplateDocument CreateDocument(long templateId, JObject payload, Func<StringValue, string> transformer)
        {
            var template = templates.First(o => o.Id == templateId);
            var templateBuffer = template.Versions.Last().Buffer;
            using var ms = new MemoryStream(templateBuffer);
            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                var fields = doc.MainDocumentPart.Document.Body
                    .Descendants<SdtRun>()
                    .Select(e => Tuple.Create(e, e.Descendants<SdtAlias>().FirstOrDefault().Val));
                foreach (var field in fields)
                {
                    var value = payload.GetValue(transformer(field.Item2))?.ToString() ?? string.Empty;
                    field.Item1
                        .Descendants<SdtContentRun>().FirstOrDefault()
                        .Descendants<Run>().FirstOrDefault()
                        .Descendants<Text>().FirstOrDefault()
                        .Text = value;
                }
            }
            var documentBuffer = ms.ToArray();
            var templateDocument = template.AddDocument(documentBuffer);
            return templateDocument;
        }
    }
}
