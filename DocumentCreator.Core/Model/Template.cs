using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class TemplateData
    {
        public string Name { get; set; }
    }

    public class Template : TemplateData
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
        [JsonIgnore]
        public byte[] Buffer { get; set; }
        public IEnumerable<TemplateField> Fields { get; set; }
    }

    public class TemplateField
    {
        public TemplateField()
        {
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsCollection { get; set; }
        public string Parent { get; set; }

        [JsonIgnore]
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Parent}.{Name} {Type} {IsCollection}";
        }
    }
}
