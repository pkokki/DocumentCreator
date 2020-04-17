using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class Template
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        [JsonIgnore]
        public byte[] Buffer { get; set; }
        public IEnumerable<TemplateField> Fields { get; set; }
    }
}
