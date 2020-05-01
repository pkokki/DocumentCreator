using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCreator
{
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly IRepository repository;

        public TemplateProcessor(IRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<Template> GetTemplates(string templateName = null)
        {
            if (templateName == null)
                return repository.GetTemplates().Select(o => Transform(o));
            else
                return repository.GetTemplateVersions(templateName).Select(o => Transform(o));
        }

        public TemplateDetails GetTemplate(string templateName, string templateVersion = null)
        {
            var content = repository.GetTemplate(templateName, templateVersion);
            if (content == null) return null;
            var fields = OpenXmlWordProcessing.FindTemplateFields(content.Buffer);
            var template = TransformFull(content, fields);
            return template;
        }

        public async Task<TemplateDetails> CreateTemplate(TemplateData templateData, Stream bytes)
        {
            templateData = templateData ?? throw new ArgumentNullException(nameof(templateData));
            var templateName = templateData.TemplateName ?? throw new ArgumentNullException(nameof(templateData.TemplateName));

            IEnumerable<TemplateField> fields;
            try
            {
                fields = OpenXmlWordProcessing.FindTemplateFields(bytes);
            }
            catch (FileFormatException)
            {
                throw new ArgumentException(nameof(bytes));
            }
            var content = await repository.CreateTemplate(templateName, bytes);
            var conversion = OpenXmlWordConverter.ConvertToHtml(bytes, content.Name);
            repository.SaveHtml(content.Name, null, conversion.Images);
            return TransformFull(content, fields);
        }

        private TemplateDetails TransformFull(ContentItem contentItem, IEnumerable<TemplateField> fields)
        {
            var template = new TemplateDetails();
            Transform(contentItem, template);
            template.Buffer = contentItem.Buffer;
            template.Fields = fields;
            return template;
        }
        private Template Transform(ContentItemSummary contentItem)
        {
            var template = new Template();
            Transform(contentItem, template);
            return template;
        }

        private void Transform(ContentItemSummary contentItem, Template template)
        {
            var parts = contentItem.Name.Split('_');
            template.FileName = contentItem.FileName;
            template.TemplateName = parts[0];
            template.Version = parts[1];
            template.Timestamp = contentItem.Timestamp;
            template.Size = contentItem.Size;
        }
    }
}
