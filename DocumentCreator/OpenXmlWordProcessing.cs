using System;
using System.Collections.Generic;
using System.Linq;
using DocumentCreator.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;

namespace DocumentCreator
{
    public static class OpenXmlWordProcessing
    {
        public static IEnumerable<TemplateField> GetTemplateFields(WordprocessingDocument doc)
        {
            var sdts = doc.MainDocumentPart.Document.Body
                .Descendants<SdtElement>()
                .Where(o => !o.Elements<SdtProperties>().First().Elements<SdtRepeatedSectionItem>().Any())
                .ToList();

            var fields = new List<TemplateField>();
            foreach (var sdt in sdts)
            {
                var sdtProperties = sdt.Elements<SdtProperties>().First();

                var field = new TemplateField()
                {
                    Name = sdtProperties.Elements<SdtAlias>().FirstOrDefault()?.Val?.ToString()
                        ?? sdtProperties.Elements<SdtId>().FirstOrDefault()?.Val,
                    IsCollection = sdtProperties.Elements<SdtRepeatedSection>().Any(),
                    Content = sdt.Elements<SdtContentBlock>().FirstOrDefault()?.InnerText,
                    Type = sdt.GetType().Name
                };
                var parent = sdt.Ancestors<SdtElement>()
                    .FirstOrDefault(o => !o.Elements<SdtProperties>().First().Elements<SdtRepeatedSectionItem>().Any());
                if (parent != null)
                {
                    var parentProperties = parent.Elements<SdtProperties>().First();
                    field.Parent = parentProperties.Elements<SdtAlias>().FirstOrDefault()?.Val?.ToString() 
                        ?? parentProperties.Elements<SdtId>().FirstOrDefault()?.Val;
                }
                fields.Add(field);
            }
            return fields;
        }

        public static void CreateDocument(WordprocessingDocument doc, JObject payload, Func<StringValue, string> transformer)
        {
            var fields = doc.MainDocumentPart.Document.Body
                    .Descendants<SdtRun>()
                    .Select(e => System.Tuple.Create(e, e.Descendants<SdtAlias>().FirstOrDefault().Val));
            foreach (var field in fields)
            {
                var value = payload.GetValue(transformer(field.Item2))?.ToString() ?? string.Empty;
                field.Item1
                    .Descendants<SdtContentRun>().FirstOrDefault()
                    .Descendants<Run>().FirstOrDefault()
                    .Descendants<Text>().FirstOrDefault()
                    .Text = value;
            }
        }

        public static void ReplaceContentControl(WordprocessingDocument doc, string name, string text)
        {
            throw new NotImplementedException();
        }
    }
}
