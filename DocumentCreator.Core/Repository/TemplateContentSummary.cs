using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class TemplateContentSummary : ContentItemSummary
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
    }
}
