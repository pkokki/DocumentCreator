using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class TemplateContent : TemplateContentSummary
    {
        public Stream Buffer { get; set; }
    }
}
