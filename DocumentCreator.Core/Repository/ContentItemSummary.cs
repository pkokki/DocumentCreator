using System;

namespace DocumentCreator.Core.Repository
{
    public class ContentItemSummary
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
