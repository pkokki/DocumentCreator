using System;
using System.Text.Json.Serialization;

namespace DocumentCreator.Core.Model
{
    public class Document
    {
        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingName { get; set; }
        public string MappingVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        [JsonIgnore]
        public byte[] Buffer { get; set; }
        public string FileName { get; set; }
    }

    public class DocumentDetails : Document
    {

    }

    public class DocumentQuery : PagingParams
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingsName { get; set; }
        public string MappingsVersion { get; set; }
    }
}
