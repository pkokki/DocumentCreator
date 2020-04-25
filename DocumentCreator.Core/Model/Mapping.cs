using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class MappingData
    {
        public string MappingName { get; set; }
        public string TemplateName { get; set; }
    }
    public class Mapping : MappingData
    {
        public string MappingVersion { get; set; }
        public string TemplateVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        
        [JsonIgnore]
        public byte[] Buffer { get; set; }
        public string FileName { get; set; }
    }

    public class MappingDetails : Mapping
    {
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<MappingSource> Sources { get; set; }
    }

    public class MappingExpression
    {
        public string Name { get; set; }
        public string Cell { get; set; }
        public string Expression { get; set; }
        public string Parent { get; set; }
        public bool IsCollection { get; set; }
        public string Content { get; set; }
    }

    public class MappingSource
    {
        public string Name { get; set; }
        public string Cell { get; set; }
        public JObject Payload { get; set; }
    }

    public class MappingStats
    {
        public string MappingName { get; set; }
        public string TemplateName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Templates { get; set; }
        public int Documents { get; set; }
    }
}
