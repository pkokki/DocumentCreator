using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace DocumentCreator.Model
{
    public class TemplateField
    {
        public TemplateField()
        {
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsCollection { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Parent}.{Name} {Type} {IsCollection}";
        }
    }
}
