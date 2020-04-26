using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IRepository repository;

        public DocumentProcessor(IRepository repository)
        {
            this.repository = repository;
        }

        public PagedResults<Document> GetDocuments(DocumentQuery query)
        {
            var documents = repository.GetDocuments(query.TemplateName, query.TemplateVersion, query.MappingsName, query.MappingsVersion);
            var orderBy = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(query.OrderBy ?? nameof(Document.DocumentId));
            return documents
                .Select(o => Transform(o))
                .AsQueryable()
                .CreatePagedResults(query.Page, query.PageSize, orderBy, !query.Descending);
        }

        public DocumentDetails CreateDocument(string templateName, string mappingName, DocumentPayload payload)
        {
            var template = repository.GetLatestTemplate(templateName);

            byte[] mappingBytes = null;
            if (mappingName != null)
            {
                var mapping = repository.GetLatestMapping(templateName, null, mappingName);
                mappingBytes = mapping.Buffer;
            }
            var documentBytes = CreateDocument(template.Buffer, mappingBytes, payload);
            var document = repository.CreateDocument(templateName, mappingName, documentBytes);

            var templateVersionName = template.Name;
            var conversion = OpenXmlWordConverter.ConvertToHtml(document.Buffer, templateVersionName, document.Name);
            repository.SaveHtml(document.Name, conversion.Html, conversion.Images);
            return TransformFull(document);
        }

        public byte[] CreateDocument(byte[] templateBytes, byte[] mappingBytes, DocumentPayload payload)
        {
            var sources = payload?.Sources != null ? new List<MappingSource>(payload.Sources) : new List<MappingSource>();
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);
            var templateFieldExpressions = mappingBytes != null
                ? OpenXmlSpreadsheet.GetTemplateFieldExpressions(mappingBytes, sources)
                : BuildIdentityExpressions(templateFields, payload);
            var results = CreateDocumentInternal(templateFields, templateFieldExpressions, sources);
            var contentControlData = BuildContentControlData(templateFields, results);
            return MergeTemplateWithMappings(contentControlData, templateBytes);
        }

        private IEnumerable<MappingExpression> BuildIdentityExpressions(IEnumerable<TemplateField> templateFields, DocumentPayload payload)
        {
            var sourceName = payload?.Sources?.FirstOrDefault()?.Name ?? "X";
            return templateFields.Select((o, index) => new MappingExpression()
            {
                Name = o.Name,
                Cell = o.Name,
                Content = o.Content,
                IsCollection = o.IsCollection,
                Parent = o.Parent,
                Expression = $"=SOURCE(\"{sourceName}\",{o.Name})"
            });
        }

        public IEnumerable<EvaluationResult> CreateDocumentInMem(byte[] templateBytes, byte[] mappingBytes, DocumentPayload payload)
        {
            var sources = payload?.Sources != null ? new List<MappingSource>(payload.Sources) : new List<MappingSource>();
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);
            var templateFieldExpressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(mappingBytes, sources);
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

        private IEnumerable<EvaluationResult> CreateDocumentInternal(IEnumerable<TemplateField> templateFields,
            IEnumerable<MappingExpression> templateFieldExpressions,
            IEnumerable<MappingSource> sources)
        {

            var expressions = new List<MappingExpression>();
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


        private Document Transform(ContentItemSummary content)
        {
            var document = new Document();
            Transform(content, document);
            return document;
        }

        private DocumentDetails TransformFull(ContentItem content)
        {
            if (content != null)
            {
                var document = new DocumentDetails();
                Transform(content, document);
                document.Buffer = content.Buffer;
                return document;
            }
            return null;
        }

        private void Transform(ContentItemSummary content, Document document)
        {
            var parts = content.Name.Split('_');
            document.TemplateName = parts[0];
            document.TemplateVersion = parts[1];
            document.MappingName = parts[2];
            document.MappingVersion = parts[3];
            document.DocumentId = parts[4];
            document.Timestamp = new DateTime(long.Parse(parts[4]));
            document.Size = content.Size;
            document.FileName = content.FileName;
        }
    }
}
