using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class DocumentContentSummary : ContentItemSummary
    {
        public string Identifier { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingName { get; set; }
        public string MappingVersion { get; set; }
    }
}
