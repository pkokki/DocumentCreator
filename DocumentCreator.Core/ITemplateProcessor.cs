using DocumentCreator.Core.Model;
using System.Collections.Generic;

namespace DocumentCreator.Core
{
    public interface ITemplateProcessor
    {
        IEnumerable<Template> GetTemplates(string templateName = null);
        TemplateDetails GetTemplate(string templateName, string templateVersion = null);
        TemplateDetails CreateTemplate(TemplateData template, byte[] bytes);
    }
}
