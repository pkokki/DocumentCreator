using DocumentCreator.Core.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public static class OpenXmlWordProcessing
    {
        public static IEnumerable<TemplateField> FindTemplateFields(Stream buffer)
        {
            using var doc = WordprocessingDocument.Open(buffer, false);
            return GetTemplateFields(doc);
        }

        public static Stream MergeTemplateWithMappings(IEnumerable<ContentControlData> data, Stream templateStream)
        {
            var ms = templateStream.ToMemoryStream();
            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                foreach (var item in data)
                {
                    if (item.IsRepeatingSection)
                        ProcessRepeatingSection(doc, item.Name, item.SectionItems);
                    else
                        SetContentControlContent(doc, item.Name, item.Text);
                }
            }
            return ms;
        }



        private static IEnumerable<TemplateField> GetTemplateFields(WordprocessingDocument doc)
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
                    //Type = sdt.GetType().Name
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

        private static OpenXmlCompositeElement FindSdtContent(SdtElement sdt, string name)
        {
            OpenXmlCompositeElement sdtContent = sdt.Elements<SdtContentRun>().FirstOrDefault();
            if (sdtContent == null)
                sdtContent = sdt.Elements<SdtContentBlock>().FirstOrDefault();
            if (sdtContent == null)
                sdtContent = sdt.Elements<SdtContentRow>().FirstOrDefault();
            if (sdtContent == null)
                sdtContent = sdt.Elements<SdtContentCell>().FirstOrDefault();
            if (sdtContent == null)
                throw new InvalidOperationException($"[{name}] Νο content element: {string.Join(", ", sdt.ChildElements.Select(o => o.GetType()))}");
            return sdtContent;
        }

        private static void RemoveContentControlContent(WordprocessingDocument doc, string name)
        {
            var sdt = FindSdt(doc.MainDocumentPart.Document.Body, name);

            if (sdt != null)
                sdt.Remove();
        }

        private static string ShowContentControlContent(WordprocessingDocument doc, string name)
        {
            var sdt = FindSdt(doc.MainDocumentPart.Document.Body, name);
            if (sdt != null)
            {
                var sdtContent = FindSdtContent(sdt, name);
                return sdtContent.InnerText;
            }
            return null;
        }

        private static SdtElement FindSdt(OpenXmlCompositeElement parent, string name)
        {
            return parent
                .Descendants<SdtElement>()
                .Where(o => !o.Elements<SdtProperties>().First().Elements<SdtRepeatedSectionItem>().Any())
                .FirstOrDefault(o => ResolveTemplateFieldName(o.Elements<SdtProperties>().First()) == name);
        }

        private static void ProcessRepeatingSection(WordprocessingDocument doc, string parentName,
            Dictionary<string, IEnumerable<string>> sectionItems)
        {
            var parentSdt = FindSdt(doc.MainDocumentPart.Document.Body, parentName);
            var sdtContent = FindSdtContent(parentSdt, parentName);
            if (sdtContent.ChildElements.Count != 1)
                throw new NotImplementedException($"[{parentName}] Can not handle repeating sections with {sdtContent.ChildElements.Count} elements in content");
            var sourceRow = sdtContent.FirstChild;
            SdtElement newRow = (SdtElement)sourceRow;
            var count = sectionItems.First().Value.Count();
            for (var i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    newRow = (SdtElement)sourceRow.CloneNode(true);
                    newRow.SdtProperties?.Elements<SdtId>().FirstOrDefault()?.Remove();
                    sourceRow.InsertAfterSelf(newRow);
                    sourceRow = newRow;
                }
                foreach (var kvp in sectionItems)
                {
                    var childSdt = FindSdt(newRow, kvp.Key);
                    var childSdtContent = FindSdtContent(childSdt, kvp.Key);
                    SetTextElement(childSdtContent, kvp.Key, kvp.Value.ElementAt(i));
                }
            }
        }

        private static void SetTextElement(OpenXmlCompositeElement elem, string name, string text)
        {
            var textElem = elem.Descendants<Text>().FirstOrDefault();
            if (textElem == null && elem.ChildElements.Count > 0)
                throw new InvalidOperationException($"[{name}] No text element: {string.Join(", ", elem.ChildElements.Select(o => o.GetType()))}");
            if (textElem == null)
            {
                textElem = new Text();
                elem.Append(
                    new Paragraph(
                        new Run(
                            //new RunProperties() { Languages = new Languages() { Val = "el-GR" } },
                            textElem)));
            }
            textElem.Text = text;
        }

        private static void SetContentControlContent(WordprocessingDocument doc, string name, string text)
        {
            if (text == "#HIDE_CONTENT#")
            {
                RemoveContentControlContent(doc, name);
            }
            else if (text == "#SHOW_CONTENT#")
            {
                ShowContentControlContent(doc, name);
            }
            else
            {
                var sdt = FindSdt(doc.MainDocumentPart.Document.Body, name);
                var sdtContent = FindSdtContent(sdt, name);
                SetTextElement(sdtContent, name, text);
            }
        }
    }
}
