using System;

namespace DocumentCreator.Core.Repository
{
    public class ContentItemStats
    {
        public string MappingName { get; set; }
        public string TemplateName { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Templates { get; set; }
        public int Documents { get; set; }
    }
}
