using DocumentCreator.ExcelFormulaParser;
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

        public byte[] CreateMappingForTemplate(byte[] emptyMapping, string templateName, string mappingsName, string testUrl, byte[] templateBytes)
        {
            using var ms = new MemoryStream(templateBytes);
            using var templateDoc = WordprocessingDocument.Open(ms, false);
            var templateFields = OpenXmlWordProcessing.GetTemplateFields(templateDoc);

            using var mappingsStream = new MemoryStream();
            mappingsStream.Write(emptyMapping, 0, emptyMapping.Length);
            using (SpreadsheetDocument mappingsDoc = SpreadsheetDocument.Open(mappingsStream, true))
            {
                OpenXmlSpreadsheet.FillMappingsSheet(mappingsDoc, templateName, mappingsName, templateFields, testUrl);
            }
            var excelBytes = mappingsStream.ToArray();
            return excelBytes;
        }

        public byte[] CreateDocument(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var sources = new Dictionary<string, JToken>
            {
                { "RQ", payload }
            };
            var templateFields = FindTemplateFields(templateBytes);
            var templateFieldExpressions = GetTemplateFieldExpressions(mappingBytes);
            var results = CreateDocumentInternal(templateFields, templateFieldExpressions, sources);
            var contentControlData = BuildContentControlData(templateFields, results);
            return MergeTemplateWithMappings(contentControlData, templateBytes);
        }

        public IEnumerable<EvaluationResult> CreateDocumentInMem(byte[] templateBytes, byte[] mappingBytes, JObject payload)
        {
            var sources = new Dictionary<string, JToken>
            {
                { "RQ", payload }
            };
            var templateFields = FindTemplateFields(templateBytes);
            var templateFieldExpressions = GetTemplateFieldExpressions(mappingBytes);
            var results = CreateDocumentInternal(templateFields, templateFieldExpressions, sources);
            var contentControlData = BuildContentControlData(templateFields, results);
            MergeTemplateWithMappings(contentControlData, templateBytes);
            return results;
        }

        private byte[] MergeTemplateWithMappings(IEnumerable<ContentControlData> data, byte[] templateBytes)
        {
            using var ms = new MemoryStream();
            ms.Write(templateBytes, 0, templateBytes.Length);
            using (var doc = WordprocessingDocument.Open(ms, true))
            {
                foreach (var item in data)
                {
                    if (item.IsRepeatingSection)
                        OpenXmlWordProcessing.ProcessRepeatingSection(doc, item.Name, item.SectionItems);
                    else
                        OpenXmlWordProcessing.SetContentControlContent(doc, item.Name, item.Text);
                }
            }
            var documentBytes = ms.ToArray();
            return documentBytes;
        }

        public IEnumerable<TemplateFieldExpression> GetTemplateFieldExpressions(byte[] mappingBytes)
        {
            using var mappingsStream = new MemoryStream(mappingBytes);
            using var mappingsDoc = SpreadsheetDocument.Open(mappingsStream, false);
            var templateFieldExpressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(mappingsDoc);
            return templateFieldExpressions;
        }

        private IEnumerable<EvaluationResult> CreateDocumentInternal(IEnumerable<TemplateField> templateFields, 
            IEnumerable<TemplateFieldExpression> templateFieldExpressions, 
            IDictionary<string, JToken> sources)
        {

            var expressions = new List<TemplateFieldExpression>();
            foreach (var templateField in templateFields)
            {
                var expression = templateFieldExpressions.FirstOrDefault(m => m.Name == templateField.Name);
                if (expression != null)
                    expressions.Add(expression);
            }
            var processor = new ExpressionEvaluator(Language.Invariant, Language.ElGr);
            var results = processor.Evaluate(expressions, sources);
            return results;
        }

        private IEnumerable<ContentControlData> BuildContentControlData(IEnumerable<TemplateField> templateFields, IEnumerable<EvaluationResult> results)
        {
            var contentControlData = new List<ContentControlData>();
            foreach (var templateField in templateFields.ToList())
            {
                if (templateField.IsCollection)
                {
                    var sectionItems = new Dictionary<string, IEnumerable<string>>();
                    var children = templateFields.Where(o => o.Parent == templateField.Name);
                    foreach (var child in children)
                    {
                        var result = results.FirstOrDefault(o => o.Name == child.Name);
                        IEnumerable<string> texts;
                        if (result.Error != null)
                        {
                            texts = new List<string> { result.Error ?? result.Text };
                        }
                        else
                        {
                            if (result.Value is IEnumerable<ExcelValue> list)
                                texts = list.Select(o => o.Text);
                            else
                                texts = new List<string> { result.Error ?? result.Text };
                        }
                        sectionItems[child.Name] = texts;
                    }
                    contentControlData.Add(new ContentControlData(templateField.Name, sectionItems));
                }
                else if (templateField.Parent == null)
                {
                    var result = results.FirstOrDefault(o => o.Name == templateField.Name);
                    var text = result.Error ?? result.Text;
                    contentControlData.Add(new ContentControlData(templateField.Name, text));
                }
            }
            return contentControlData;
        }
    }
}
