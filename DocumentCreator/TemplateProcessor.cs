using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentCreator.Model;
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public class TemplateProcessor
    {
        public IEnumerable<TemplateField> FindTemplateFields(byte[] buffer)
        {
            using var ms = new MemoryStream(buffer);
            using var doc = WordprocessingDocument.Open(ms, false);
            return OpenXmlWordProcessing.GetTemplateFields(doc);
        }

        public byte[] CreateMappingForTemplate(byte[] emptyMapping, string mappingsName, string testUrl, byte[] templateBytes)
        {
            using var ms = new MemoryStream(templateBytes);
            using var templateDoc = WordprocessingDocument.Open(ms, false);
            var templateFields = OpenXmlWordProcessing.GetTemplateFields(templateDoc);

            using var mappingsStream = new MemoryStream();
            mappingsStream.Write(emptyMapping, 0, emptyMapping.Length);
            using (SpreadsheetDocument mappingsDoc = SpreadsheetDocument.Open(mappingsStream, true))
            {
                OpenXmlSpreadsheet.FillMappingsSheet(mappingsDoc, mappingsName, templateFields, testUrl);
            }
            var excelBytes = mappingsStream.ToArray();
            return excelBytes;
        }

        public byte[] CreateDocument(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var templateFieldExpressions = CreateDocumentInternal(templateBytes, mappingBytes, payload);
            return MergeTemplateWithMappings(templateFieldExpressions, templateBytes);
        }

        public IEnumerable<TemplateFieldExpression> CreateDocumentInMem(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var templateFieldExpressions = CreateDocumentInternal(templateBytes, mappingBytes, payload);
            MergeTemplateWithMappings(templateFieldExpressions, templateBytes);
            return templateFieldExpressions;
        }

        private byte[] MergeTemplateWithMappings(IEnumerable<TemplateFieldExpression> templateFieldExpressions, byte[] templateBytes)
        {
            using var ms = new MemoryStream();
            ms.Write(templateBytes, 0, templateBytes.Length);
            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                foreach (var templateFieldExpression in templateFieldExpressions.ToList())
                {
                    if (templateFieldExpression.IsCollection)
                    {
                        // Handle this and child template fields
                        var childValues = new Dictionary<string, IEnumerable<string>>();
                        templateFieldExpressions
                            .Where(o => o.Parent == templateFieldExpression.Name)
                            .ToList()
                            .ForEach(o =>
                            {
                                childValues[o.Name] = o.Result.Rows;
                                o.Result.Value = new JArray(o.Result.Rows).ToString(Newtonsoft.Json.Formatting.None).Replace("\"", "'");
                            });
                        if (templateFieldExpression.Result.ChildRows == 0)
                            throw new InvalidOperationException($"[{templateFieldExpression.Name}]: Collection is empty");
                        OpenXmlWordProcessing.ProcessRepeatingSection(doc, templateFieldExpression.Name,
                            templateFieldExpression.Result.ChildRows, childValues);

                    }
                    else if (!string.IsNullOrEmpty(templateFieldExpression.Parent))
                    {
                        // Do nothing, handled by parent
                    }
                    else
                    {
                        var text = templateFieldExpression.Result.Error ?? templateFieldExpression.Result.Value;
                        templateFieldExpression.Result.Value
                            = OpenXmlWordProcessing.SetContentControlContent(doc, templateFieldExpression.Name, text);
                    }
                }
            }
            var documentBytes = ms.ToArray();
            return documentBytes;
        }

        private IEnumerable<TemplateFieldExpression> CreateDocumentInternal(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var sources = new Dictionary<string, JToken>
            {
                { "RQ", payload }
            };

            using var mappingsStream = new MemoryStream(mappingBytes);
            using var mappingsDoc = SpreadsheetDocument.Open(mappingsStream, false);
            var templateFieldExpressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(mappingsDoc);

            using var ms = new MemoryStream(templateBytes);
            using var doc = WordprocessingDocument.Open(ms, false);
            var templateFields = OpenXmlWordProcessing.GetTemplateFields(doc);

            var processor = new ExpressionEvaluator(Language.Invariant, Language.ElGr);
            foreach (var templateField in templateFields)
            {
                var templateFieldExpression = templateFieldExpressions.FirstOrDefault(m => m.Name == templateField.Name);
                if (templateFieldExpression != null)
                {
                    templateFieldExpression.Result = processor.Evaluate(0, "=" + templateFieldExpression.Expression, sources);
                }
            }
            return templateFieldExpressions;
        }
    }
}
