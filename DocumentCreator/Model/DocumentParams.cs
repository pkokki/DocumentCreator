using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class DocumentParams : PagingParams
    {
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
        public string MappingsName { get; set; }
        public string MappingsVersion { get; set; }
    }
}
