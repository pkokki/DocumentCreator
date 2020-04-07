using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace DocumentCreator.Model
{
    public class TemplateField
    {
        public TemplateField(string name)
        {
            this.Name = name;
        }

        public TemplateField(SdtElement source)
        {
            var sdtProperties = source.Elements<SdtProperties>().First();

            this.Name = sdtProperties.Elements<SdtAlias>().FirstOrDefault()?.Val;
            this.Name ??= sdtProperties.Elements<SdtId>().FirstOrDefault()?.Val;
            this.Content = source.Elements<SdtContentBlock>().FirstOrDefault()?.InnerText;

            var parent = source.Ancestors<SdtElement>().FirstOrDefault();
            if (parent != null)
            {
                var parentProperties = parent.Elements<SdtProperties>().First();
                this.Parent = parentProperties.Elements<SdtAlias>().FirstOrDefault()?.Val;
                this.Parent ??= parentProperties.Elements<SdtId>().FirstOrDefault()?.Val;
            }


            this.Type = source.GetType().Name;
            this.IsCollection = sdtProperties.Elements<SdtRepeatedSectionItem>().Any();
        }

        public string Name { get; }
        public string Content { get; }
        public bool IsCollection { get; }
        public string Parent { get; }
        public string Type { get; }

        public override string ToString()
        {
            return $"{Parent}.{Name} {Type} {IsCollection}";
        }
    }
}
