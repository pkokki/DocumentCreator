using System.Collections.Generic;

namespace DocumentCreator
{
    public class ContentControlData
    {
        public ContentControlData(string name, string text)
        {
            Name = name;
            Text = text;
        }

        public ContentControlData(string name, Dictionary<string, IEnumerable<string>> sectionItems)
        {
            Name = name;
            IsRepeatingSection = true;
            SectionItems = sectionItems;
        }

        public string Name { get; }
        public string Text { get; }
        public bool IsRepeatingSection { get; }
        public Dictionary<string, IEnumerable<string>> SectionItems { get; }
    }
}
