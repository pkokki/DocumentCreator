using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentCreator.Core.Repository
{
    public class DocumentContent : DocumentContentSummary
    {
        public Stream Buffer { get; set; }
    }
}
