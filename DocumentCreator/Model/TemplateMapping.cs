using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class TemplateMapping
    {
        public string MappingName { get; set; }
        public string MappingVersion { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        [JsonIgnore]
        public byte[] Buffer { get; set; }
        public IEnumerable<TemplateFieldExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }
}
