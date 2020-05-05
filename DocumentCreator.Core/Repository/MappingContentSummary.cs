using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class MappingContentSummary : ContentItemSummary
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingName { get; set; }
        public string MappingVersion { get; set; }
    }
}
