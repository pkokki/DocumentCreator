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
        private readonly IHtmlRepository htmlRepository;

        public TemplateProcessor(IRepository repository, IHtmlRepository htmlRepository)
        {
            this.repository = repository;
            this.htmlRepository = htmlRepository;
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

            if (htmlRepository != null)
            {
                var conversion = OpenXmlWordConverter.ConvertToHtml(bytes, content.Name);
                htmlRepository.SaveHtml(content.Name, null, conversion.Images);
            }
            return TransformFull(content, fields);
        }

        private TemplateDetails TransformFull(TemplateContent contentItem, IEnumerable<TemplateField> fields)
        {
            var template = new TemplateDetails();
            Transform(contentItem, template);
            template.Buffer = contentItem.Buffer;
            template.Fields = fields;
            return template;
        }
        private Template Transform(TemplateContentSummary contentItem)
        {
            var template = new Template();
            Transform(contentItem, template);
            return template;
        }

        private void Transform(TemplateContentSummary contentItem, Template template)
        {
            template.FileName = contentItem.FileName;
            template.TemplateName = contentItem.TemplateName;
            template.Version = contentItem.TemplateVersion;
            template.Timestamp = contentItem.Timestamp;
            template.Size = contentItem.Size;
        }
    }
}
