using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using DocumentCreator.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<MappingDetails> GetMapping(string templateName, string templateVersion, string mappingName, string mappingVersion = null)
        {
            ContentItem content;
            if (mappingVersion == null)
                content = repository.GetLatestMapping(templateName, templateVersion, mappingName);
            else
                content = await repository.GetMapping(templateName, templateVersion, mappingName, mappingVersion);
            return TransformFull(content);
        }

        public async Task<MappingDetails> CreateMapping(string templateName, string mappingName, string testEvaluationsUrl)
        {
            var template = repository.GetLatestTemplate(templateName);
            if (template == null)
                return null;

            var emptyMappingBuffer = new MemoryStream(Resources.empty_mappings_prod_xlsm);

            var bytes = CreateMappingForTemplate(template.Buffer, emptyMappingBuffer, templateName, mappingName, testEvaluationsUrl);
            return await CreateMapping(templateName, mappingName, bytes);
        }

        public async Task<MappingDetails> CreateMapping(string templateName, string mappingName, Stream bytes)
        {
            var content = await repository.CreateMapping(templateName, mappingName, bytes);
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
