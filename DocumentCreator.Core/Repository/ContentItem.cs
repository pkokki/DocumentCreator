using System.IO;

namespace DocumentCreator.Core.Repository
{
    public class ContentItem : ContentItemSummary
    {
        public Stream Buffer { get; set; }
    }
}
