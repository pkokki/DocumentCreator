using DocumentCreator.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            return OpenXmlWordProcessing.GetTemplateFields(doc);
        }

        public byte[] CreateMappingsForTemplate(string emptyMappingsPath, string mappingsName, string testUrl, byte[] templateBytes)
        {
            using var ms = new MemoryStream(templateBytes);
            using var templateDoc = WordprocessingDocument.Open(ms, false);
            var templateFields = OpenXmlWordProcessing.GetTemplateFields(templateDoc);

            var emptyBytes = File.ReadAllBytes(emptyMappingsPath);
            using var mappingsStream = new MemoryStream();
            mappingsStream.Write(emptyBytes, 0, emptyBytes.Length);
            using (SpreadsheetDocument mappingsDoc = SpreadsheetDocument.Open(mappingsStream, true))
            {
                OpenXmlSpreadsheet.FillMappingsSheet(mappingsDoc, mappingsName, templateFields, testUrl);
            }
            var excelBytes = mappingsStream.ToArray();
            return excelBytes;
        }

        public byte[] CreateDocument(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var transformations = Transform(templateBytes, mappingBytes, payload);

            using var ms = new MemoryStream();
            ms.Write(templateBytes, 0, templateBytes.Length);
            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                foreach (var transformation in transformations)
                {
                    var text = transformation.Result.Error ?? transformation.Result.Value;
                    OpenXmlWordProcessing.ReplaceContentControl(doc, transformation.Name, text);
                }
            }
            var documentBytes = ms.ToArray();
            return documentBytes;
        }
        public IEnumerable<Transformation> Transform(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var sources = new Dictionary<string, JToken>();
            sources.Add("RQ", payload);

            using var mappingsStream = new MemoryStream(mappingBytes);
            using var mappingsDoc = SpreadsheetDocument.Open(mappingsStream, false);
            var transformations = OpenXmlSpreadsheet.GetTransformations(mappingsDoc);

            using var ms = new MemoryStream(templateBytes);
            using var doc = WordprocessingDocument.Open(ms, false);
            var templateFields = OpenXmlWordProcessing.GetTemplateFields(doc);

            var processor = new TransformProcessor(CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("el-GR"));
            foreach (var templateField in templateFields)
            {
                var transformation = transformations.FirstOrDefault(m => m.Name == templateField.Name);
                if (transformation != null)
                {
                    transformation.Result = processor.Evaluate(0, "=" + transformation.Expression, sources);
                }
            }
            return transformations;
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
                OpenXmlWordProcessing.CreateDocument(doc, payload, transformer);
            }
            var documentBuffer = ms.ToArray();
            var templateDocument = template.AddDocument(documentBuffer);
            return templateDocument;
        }
    }
}
