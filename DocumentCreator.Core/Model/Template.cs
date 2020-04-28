using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class TemplateData
    {
        public string TemplateName { get; set; }
    }

    public class Template : TemplateData
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public DateTime Timestamp { get; set; }
        public long Size { get; set; }
    }

    public class TemplateDetails : Template
    {
        public IEnumerable<TemplateField> Fields { get; set; }

        [JsonIgnore]
        public byte[] Buffer { get; set; }
    }

    public class TemplateField
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsCollection { get; set; }
        public string Parent { get; set; }
    }
}
