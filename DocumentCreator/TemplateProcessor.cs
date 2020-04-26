using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator
{
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly IRepository repository;

        public TemplateProcessor(IRepository repository)
        {
            this.repository = repository;
        }

        private TemplateDetails TransformFull(ContentItem contentItem)
        {
            var template = new TemplateDetails();
            Transform(contentItem, template);
            template.Buffer = contentItem.Buffer;
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
            template.Timestamp = new DateTime(long.Parse(parts[1]));
            template.Size = contentItem.Size;
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
            var template = TransformFull(repository.GetTemplate(templateName, templateVersion));
            template.Fields = OpenXmlWordProcessing.FindTemplateFields(template.Buffer);
            return template;
        }

        public TemplateDetails CreateTemplate(TemplateData templateData, byte[] bytes)
        {
            var contentItem = repository.CreateTemplate(templateData.TemplateName, bytes);

            var templateVersionName = contentItem.Name;
            var conversion = OpenXmlWordConverter.ConvertToHtml(bytes, templateVersionName);
            repository.SaveHtml(templateVersionName, null, conversion.Images);

            return TransformFull(contentItem);
        }
    }
}
