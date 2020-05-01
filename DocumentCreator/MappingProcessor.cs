using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
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

            var bytes = CreateMappingForTemplate(template.Buffer, emptyMappingBuffer, templateName, mappingName, testEvaluationsUrl);
            return CreateMapping(templateName, mappingName, bytes);
        }

        public MappingDetails CreateMapping(string templateName, string mappingName, Stream bytes)
        {
            var content = repository.CreateMapping(templateName, mappingName, bytes);
            return TransformFull(content);
        }

        public Stream CreateMappingForTemplate(Stream templateBytes, Stream mappingBytes, string templateName, string mappingsName, string testUrl)
        {
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);
            var excelBytes = OpenXmlSpreadsheet.FillMappingsSheet(mappingBytes, templateFields, templateName, mappingsName, testUrl);
            return excelBytes;
        }

        public EvaluationOutput Evaluate(EvaluationRequest request)
        {
            IEnumerable<TemplateField> templateFields = null;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                var template = repository.GetLatestTemplate(request.TemplateName);
                if (template == null)
                    return null;
                templateFields = OpenXmlWordProcessing.FindTemplateFields(template.Buffer);
            }

            var processor = new MappingExpressionEvaluator();
            var input = new EvaluationInput()
            {
                Fields = templateFields,
                Expressions = request.Expressions,
                Sources = request.Sources
            };
            var response = processor.Evaluate(input);
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
                var info = OpenXmlSpreadsheet.GetMappingInfo(content.Buffer, null);
                var mapping = new MappingDetails();
                Transform(content, mapping);
                mapping.Buffer = content.Buffer;
                mapping.Expressions = info.Expressions;
                mapping.Sources = info.Sources;
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
    }
}
