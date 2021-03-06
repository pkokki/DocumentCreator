﻿using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using JsonExcelExpressions;
using JsonExcelExpressions.Eval;
using JsonExcelExpressions.Lang;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCreator
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly IRepository repository;
        private readonly IHtmlRepository htmlRepository;

        public DocumentProcessor(IRepository repository, IHtmlRepository htmlRepository)
        {
            this.repository = repository;
            this.htmlRepository = htmlRepository;
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

        public DocumentDetails GetDocument(string documentId)
        {
            var document = repository.GetDocument(documentId);
            return TransformFull(document);
        }

        public async Task<DocumentDetails> CreateDocument(string templateName, string mappingName, DocumentPayload payload)
        {
            var template = repository.GetLatestTemplate(templateName);

            Stream mappingBytes = null;
            if (mappingName != null)
            {
                var mapping = repository.GetLatestMapping(templateName, null, mappingName);
                mappingBytes = mapping.Buffer;
            }
            var documentBytes = CreateDocument(template.Buffer, mappingBytes, payload);
            var document = await repository.CreateDocument(templateName, mappingName, documentBytes);

            if (htmlRepository != null)
            {
                var pageTitle = template.Name;
                var conversion = OpenXmlWordConverter.ConvertToHtml(document.Buffer, pageTitle, document.Name);
                htmlRepository.SaveHtml(document.Name, conversion.Html, conversion.Images);
            }
            return TransformFull(document);
        }

        public Stream CreateDocument(Stream templateBytes, Stream mappingBytes, DocumentPayload payload)
        {
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);
            MappingInfo mappingInfo;
            if (mappingBytes != null)
                mappingInfo = OpenXmlSpreadsheet.GetMappingInfo(mappingBytes, payload.Sources);
            else
                mappingInfo = OpenXmlSpreadsheet.BuildIdentityExpressions(templateFields, payload.Sources);
            var results = CreateDocumentInternal(templateFields, mappingInfo.Expressions, mappingInfo.Sources);
            var contentControlData = BuildContentControlData(templateFields, results);
            return OpenXmlWordProcessing.MergeTemplateWithMappings(contentControlData, templateBytes);
        }
        
        private IEnumerable<EvaluationResult> CreateDocumentInternal(IEnumerable<TemplateField> templateFields,
            IEnumerable<MappingExpression> templateFieldExpressions,
            IEnumerable<EvaluationSource> sources)
        {
            var expressions = new List<MappingExpression>();
            foreach (var templateField in templateFields)
            {
                var expression = templateFieldExpressions.FirstOrDefault(m => m.Name == templateField.Name);
                if (expression != null)
                    expressions.Add(expression);
            }
            var processor = new MappingExpressionEvaluator();
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
                    var text = result != null ? (result.Error ?? result.Text) : string.Empty;
                    contentControlData.Add(new ContentControlData(templateField.Name, text));
                }
            }
            return contentControlData;
        }

        private Document Transform(DocumentContentSummary content)
        {
            var document = new Document();
            Transform(content, document);
            return document;
        }

        private DocumentDetails TransformFull(DocumentContent content)
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

        private void Transform(DocumentContentSummary content, Document document)
        {
            document.TemplateName = content.TemplateName;
            document.TemplateVersion = content.TemplateVersion;
            document.MappingName = content.MappingName;
            document.MappingVersion = content.MappingVersion;
            document.DocumentId = content.Identifier;
            document.Timestamp = content.Timestamp;
            document.Size = content.Size;
            document.FileName = content.FileName;
            if (htmlRepository != null)
                document.Url = htmlRepository.GetUrl(content.Name);
        }
    }
}
