using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class MappingContent : MappingContentSummary
    {
        public Stream Buffer { get; set; }
    }
}
