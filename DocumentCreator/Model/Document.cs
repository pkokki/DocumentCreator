using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
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
    }
}
