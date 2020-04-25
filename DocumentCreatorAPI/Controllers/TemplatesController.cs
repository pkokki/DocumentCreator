using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(processor.GetTemplates());
        }

        //{ path: 'templates/:templateName', component: TemplateDetailComponent },
        [HttpGet]
        [Route("{templateName}")]
        public IActionResult GetTemplate([FromRoute]string templateName)
        {
            return Ok(processor.GetTemplate(templateName));
        }

        //{ path: 'templates/:templateName/versions', component: TemplatesTableComponent },
        [HttpGet]
        [Route("{templateName}/versions")]
        public IActionResult GetTemplateVersions([FromRoute]string templateName)
        {
            var versions = processor.GetTemplates(templateName);
            return Ok(versions);
        }

        //{ path: 'templates/:templateName/versions/:templateVersion', component: TemplateDetailComponent },
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}")]
        public IActionResult GetTemplateVersion([FromRoute]string templateName, [FromRoute]string templateVersion)
        {
            return Ok(processor.GetTemplate(templateName, templateVersion));
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult CreateTemplate([FromBody]TemplateData templateData)
        {
            //var templateName = Request.Form["name"][0];
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
