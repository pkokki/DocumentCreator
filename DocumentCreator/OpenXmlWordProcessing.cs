using System;
using System.Collections.Generic;
using System.Linq;
using DocumentCreator.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;

namespace DocumentCreator
{
    public static class OpenXmlWordProcessing
    {
        public static IEnumerable<TemplateField> GetTemplateFields(WordprocessingDocument doc)
        {
            //var x = doc.MainDocumentPart.Document.Body.Descendants<SdtElement>();
            //var y = x.Select(o => o.GetType().Name);
            return doc.MainDocumentPart.Document.Body
                .Descendants<SdtElement>()
                .Select(e => new TemplateField(e))
                .ToList();
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
    }
}
