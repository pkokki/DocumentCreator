using DocumentCreator.Core.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreator.Core
{
    public interface ITemplateProcessor
    {
        IEnumerable<Template> GetTemplates(string templateName = null);
        TemplateDetails GetTemplate(string templateName, string templateVersion = null);
        Task<TemplateDetails> CreateTemplate(TemplateData template, Stream bytes);
    }
}
