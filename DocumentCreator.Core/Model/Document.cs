using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DocumentCreator.Core.Model
{
    public class DocumentData
    {
        public string DocumentId { get; set; }
    }

    public class Document : DocumentData
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingName { get; set; }
        public string MappingVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }

    public class DocumentDetails : Document
    {
        [JsonIgnore]
        public byte[] Buffer { get; set; }
    }

    public class DocumentQuery : PagingParams
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingsName { get; set; }
        public string MappingsVersion { get; set; }
    }

    public class DocumentPayload
    {
        public IEnumerable<MappingSource> Sources { get; set; }
    }
}
