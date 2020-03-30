using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DocumentCreator.Model
{
    public class Template
    {
        public Template(long id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Versions = new List<TemplateVersion>();
            this.Documents = new List<TemplateDocument>();
            this.TemplateMappings = new List<TemplateMapping>();
        }

        public long Id { get; }
        public List<TemplateVersion> Versions { get; }
        public List<TemplateDocument> Documents { get; }
        public List<TemplateMapping> TemplateMappings { get; }
        public string Name { get; set; }

        internal TemplateVersion AddVersion(byte[] buffer)
        {
            var version = new TemplateVersion(Versions.LastOrDefault()?.Id + 1 ?? 1, buffer);
            Versions.Add(version);
            return version;
        }

        internal TemplateDocument AddDocument(byte[] buffer)
        {
            var templateDocument = new TemplateDocument(Documents.LastOrDefault()?.Id + 1 ?? 1, buffer);
            Documents.Add(templateDocument);
            return templateDocument;
        }

        internal TemplateMapping UpsertMapping(string name, JObject transformations)
        {
            var mapping = TemplateMappings.FirstOrDefault(o => o.Name == name);
            if (mapping == null)
            {
                mapping = new TemplateMapping(name);
                TemplateMappings.Add(mapping);
            }
            mapping.Upsert(transformations);
            return mapping;
        }
    }
}
