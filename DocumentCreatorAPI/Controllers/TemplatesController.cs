using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateProcessor processor;

        public TemplatesController(ITemplateProcessor processor)
        {
            this.processor = processor;
        }

        //{ path: 'templates', component: TemplatesTableComponent },
        //{ path: 'templates/:templateName/versions', component: TemplatesTableComponent },
        [HttpGet]
        [Route("")]
        [Route("{templateName}/versions")]
        public IActionResult GetTemplateVersions([FromRoute]string templateName)
        {
            var versions = processor.GetTemplates(templateName);
            return Ok(versions);
        }

        //{ path: 'templates/:templateName', component: TemplateDetailComponent },
        //{ path: 'templates/:templateName/versions/:templateVersion', component: TemplateDetailComponent },
        [HttpGet]
        [Route("{templateName}")]
        [Route("{templateName}/versions/{templateVersion}")]
        public IActionResult GetTemplate([FromRoute]string templateName, [FromRoute]string templateVersion)
        {
            return Ok(processor.GetTemplate(templateName, templateVersion));
        }



        [HttpPost, DisableRequestSizeLimit]
        [Route("")]
        public IActionResult CreateTemplate()
        {
            var templateData = new TemplateData()
            {
                TemplateName = Request.Form["name"]
            };
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var ms = new MemoryStream();
                formFile.CopyTo(ms);
                var template = processor.CreateTemplate(templateData, ms.ToArray());
                return Ok(template);
            }
            else
                return BadRequest();
        }
    }
}
