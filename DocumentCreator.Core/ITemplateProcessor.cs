using DocumentCreator.Core.Model;
using System.Collections.Generic;

namespace DocumentCreator.Core
{
    public interface ITemplateProcessor
    {
        IEnumerable<Template> GetTemplates(string templateName = null);
        Template GetTemplate(string templateName, string templateVersion = null);
        Template CreateTemplate(TemplateData template, byte[] bytes);
    }
}
