using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using DocumentCreator.Properties;
using Newtonsoft.Json.Linq;
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
            MappingContent content;
            if (mappingVersion == null)
                content = repository.GetLatestMapping(templateName, templateVersion, mappingName);
            else
                content = await repository.GetMapping(templateName, templateVersion, mappingName, mappingVersion);
            return TransformFull(content);
        }

        public async Task<FillMappingResult> BuildMapping(FillMappingInfo info)
        {
            var template = repository.GetLatestTemplate(info.TemplateName);
            if (template == null)
                return null;

            var mapping = await GetMapping(info.TemplateName, null, info.MappingName);
            var mappingBytes = mapping != null ? mapping.Buffer : new MemoryStream(Resources.empty_mappings_prod_xlsm);

            var bytes = CreateMappingForTemplate(template.Buffer, mappingBytes, info);
            return new FillMappingResult()
            {
                FileName = $"{info.TemplateName}_{info.MappingName}.xlsm",
                Buffer = bytes
            };
        }

        public async Task<MappingDetails> CreateMapping(string templateName, string mappingName, Stream bytes)
        {
            var content = await repository.CreateMapping(templateName, mappingName, bytes);
            return TransformFull(content);
        }

        public Stream CreateMappingForTemplate(Stream templateBytes, Stream mappingBytes, FillMappingInfo info)
        {
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);
            var excelBytes = OpenXmlSpreadsheet.FillMappingsSheet(mappingBytes, templateFields, info);
            return excelBytes;
        }

        public EvaluationOutput Evaluate(EvaluationRequest request)
        {
            IEnumerable<TemplateField> templateFields = null;
            if (!string.IsNullOrEmpty(request.TemplateName))
            {
                var template = repository.GetLatestTemplate(request.TemplateName);
                if (template != null)
                    templateFields = OpenXmlWordProcessing.FindTemplateFields(template.Buffer);
            }
            if (templateFields == null)
            {
                templateFields = request.Expressions.Select(e => new TemplateField()
                {
                    Name = e.Name,
                    Parent = e.Parent,
                    IsCollection = e.IsCollection,
                    Content = e.Content
                });
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

        private Mapping Transform(MappingContentSummary content)
        {
            var mapping = new Mapping();
            Transform(content, mapping);
            return mapping;
        }

        private MappingDetails TransformFull(MappingContent content)
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

        private void Transform(MappingContentSummary content, Mapping mapping)
        {
            mapping.FileName = content.FileName;
            mapping.TemplateName = content.TemplateName;
            mapping.TemplateVersion = content.TemplateVersion;
            mapping.MappingName = content.MappingName;
            mapping.MappingVersion = content.MappingVersion;
            mapping.Timestamp = content.Timestamp;
            mapping.Size = content.Size;
        }
    }
}
