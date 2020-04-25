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

        private Template Transform(ContentItem contentItem)
        {
            var template = new Template();
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
            template.Name = parts[0];
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

        public Template GetTemplate(string templateName, string templateVersion = null)
        {
            var template = Transform(repository.GetTemplate(templateName, templateVersion));
            template.Fields = OpenXmlWordProcessing.FindTemplateFields(template.Buffer);
            return template;
        }

        public Template CreateTemplate(TemplateData templateData, byte[] bytes)
        {
            var contentItem = repository.CreateTemplate(templateData.Name, bytes);

            var templateVersionName = contentItem.Name;
            var conversion = OpenXmlWordConverter.ConvertToHtml(bytes, templateVersionName);
            repository.SaveHtml(templateVersionName, null, conversion.Images);

            return Transform(contentItem);
        }
    }
}
