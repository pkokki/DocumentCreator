using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocumentCreator
{
    public class MappingProcessor : IMappingProcessor
    {
        private readonly IRepository repository;

        public MappingProcessor(IRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<MappingStats> GetMappingStats(string mappingName = null)
        {
            var items = repository.GetMappingStats(mappingName)
                .Select(o => new MappingStats() 
                {
                    MappingName = o.MappingName,
                    TemplateName = mappingName == null ? null : o.TemplateName,
                    Timestamp = o.TimeStamp,
                    Documents = o.Documents,
                    Templates = o.Templates
                });
            return items;
        }

        public IEnumerable<Mapping> GetMappings(string templateName = null, string templateVersion = null, string mappingName = null)
        {
            var items = repository.GetMappings(templateName, templateVersion, mappingName);
            return items.Select(o => Transform(o)).ToList();
        }

        public MappingDetails GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion = null)
        {
            ContentItem content;
            if (mappingVersion == null)
                content = repository.GetLatestMapping(templateName, templateVersion, mappingName);
            else
                content = repository.GetMapping(templateName, templateVersion, mappingName, mappingVersion);
            return TransformFull(content);
        }

        public MappingDetails CreateMapping(string templateName, string mappingName, string testEvaluationsUrl)
        {
            var template = repository.GetLatestTemplate(templateName);
            if (template == null)
                return null;
            var emptyMappingBuffer = repository.GetEmptyMapping();

            var bytes = CreateMappingForTemplate(emptyMappingBuffer, templateName, mappingName, testEvaluationsUrl, template.Buffer);
            return CreateMapping(templateName, mappingName, bytes);
        }

        public MappingDetails CreateMapping(string templateName, string mappingName, byte[] bytes)
        {
            var content = repository.CreateMapping(templateName, mappingName, bytes);
            return TransformFull(content);
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

        public Evaluation Evaluate(EvaluationRequest request)
        {
            IEnumerable<TemplateField> templateFields = null;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                var template = repository.GetLatestTemplate(request.TemplateName);
                if (template == null)
                    return null;
                templateFields = OpenXmlWordProcessing.FindTemplateFields(template.Buffer);
            }

            var processor = new ExpressionEvaluator();
            var response = processor.Evaluate(request, templateFields);
            return response;
        }

        private Mapping Transform(ContentItemSummary content)
        {
            var mapping = new Mapping();
            Transform(content, mapping);
            return mapping;
        }

        private MappingDetails TransformFull(ContentItem content)
        {
            if (content != null)
            {
                var sources = new List<MappingSource>();
                var expressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(content.Buffer, sources);
                var mapping = new MappingDetails();
                Transform(content, mapping);
                mapping.Buffer = content.Buffer;
                mapping.Expressions = expressions.Select(o => Transform(o));
                mapping.Sources = sources.Select(o => Transform(o));
                return mapping;
            }
            return null;
        }

        private void Transform(ContentItemSummary content, Mapping mapping)
        {
            var parts = content.Name.Split('_');
            mapping.FileName = content.FileName;
            mapping.TemplateName = parts[0];
            mapping.TemplateVersion = parts[1];
            mapping.MappingName = parts[2];
            mapping.MappingVersion = parts[3];
            mapping.Timestamp = content.Timestamp;
            mapping.Size = content.Size;
        }

        private MappingSource Transform(MappingSource source)
        {
            return new MappingSource()
            {
                Name = source.Name,
                Cell = source.Cell,
                Payload = source.Payload
            };
        }

        private MappingExpression Transform(MappingExpression source)
        {
            return new MappingExpression()
            {
                Name = source.Name,
                Cell = source.Cell,
                Expression = source.Expression,
                Parent = source.Parent,
                IsCollection = source.IsCollection,
                Content = source.Content,
            };
        }
    }
}
