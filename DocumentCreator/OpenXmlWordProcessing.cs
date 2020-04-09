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
                    Name = ResolveTemplateFieldName(sdtProperties),
                    IsCollection = sdtProperties.Elements<SdtRepeatedSection>().Any(),
                    Content = sdt.Elements<SdtContentBlock>().FirstOrDefault()?.InnerText,
                    Type = sdt.GetType().Name
                };
                var parent = sdt.Ancestors<SdtElement>()
                    .FirstOrDefault(o => !o.Elements<SdtProperties>().First().Elements<SdtRepeatedSectionItem>().Any());
                if (parent != null)
                {
                    var parentProperties = parent.Elements<SdtProperties>().First();
                    field.Parent = ResolveTemplateFieldName(parentProperties);
                }
                fields.Add(field);
            }
            return fields;
        }

        private static string ResolveTemplateFieldName(SdtProperties sdtProperties)
        {
            return sdtProperties.Elements<SdtAlias>().FirstOrDefault()?.Val?.ToString()
                        ?? sdtProperties.Elements<SdtId>().FirstOrDefault()?.Val;
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
            if (text == "[]")
                return;
            var sdt = FindSdt(doc, name);
            var sdtContent = FindSdtContent(sdt, name);
            
            var textElem = sdtContent.Descendants<Text>().FirstOrDefault();
            if (textElem == null && sdtContent.ChildElements.Count > 0)
                throw new InvalidOperationException($"[{name}] No text element: {string.Join(", ", sdtContent.ChildElements.Select(o => o.GetType()))}");
            if (textElem == null)
            {
                textElem = new Text();
                sdtContent.Append(
                    new Paragraph(
                        new Run(
                            //new RunProperties() { Languages = new Languages() { Val = "el-GR" } },
                            textElem)));
            }
            textElem.Text = text;
        }

        private static OpenXmlCompositeElement FindSdtContent(SdtElement sdt, string name)
        {
            OpenXmlCompositeElement sdtContent = sdt.Elements<SdtContentRun>().FirstOrDefault();
            if (sdtContent == null)
                sdtContent = sdt.Elements<SdtContentBlock>().FirstOrDefault();
            if (sdtContent == null)
                sdtContent = sdt.Elements<SdtContentCell>().FirstOrDefault();
            if (sdtContent == null)
                throw new InvalidOperationException($"[{name}] Νο content element: {string.Join(", ", sdt.ChildElements.Select(o => o.GetType()))}");
            return sdtContent;
        }

        public static void RemoveContentControlContent(WordprocessingDocument doc, string name)
        {
            var sdt = FindSdt(doc, name);

            if (sdt != null)
                sdt.Remove();
        }

        public static string ShowContentControlContent(WordprocessingDocument doc, string name)
        {
            var sdt = FindSdt(doc, name);
            if (sdt != null)
            {
                var sdtContent = FindSdtContent(sdt, name);
                return sdtContent.InnerText;
            }
            return null;
        }

        private static SdtElement FindSdt(WordprocessingDocument doc, string name)
        {
            return doc.MainDocumentPart.Document.Body
                .Descendants<SdtElement>()
                .Where(o => !o.Elements<SdtProperties>().First().Elements<SdtRepeatedSectionItem>().Any())
                .FirstOrDefault(o => ResolveTemplateFieldName(o.Elements<SdtProperties>().First()) == name);
        }
    }
}
