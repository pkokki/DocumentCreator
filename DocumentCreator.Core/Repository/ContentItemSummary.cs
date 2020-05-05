using System;
using System.Collections.Generic;

namespace DocumentCreator.Core.Repository
{
    public class ContentItemSummary
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
