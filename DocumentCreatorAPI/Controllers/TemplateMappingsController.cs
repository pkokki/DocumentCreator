using DocumentCreator.Core;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplateMappingsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        public TemplateMappingsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        //{ path: 'templates/:templateName/mappings', component: MappingsTableComponent },
        [HttpGet]
        [Route("{templateName}/mappings")]
        public IActionResult GetTemplateMappings([FromRoute]string templateName)
        {
            var mappings = processor.GetMappings(templateName);
            return Ok(mappings);
        }

        //{ path: 'templates/:templateName/versions/:templateVersion/mappings', component: MappingsTableComponent },
        [HttpGet]
        [Route("{templateName}/versions/:templateVersion/mappings")]
        public IActionResult GetTemplateVersionMappings([FromRoute]string templateName, [FromRoute]string templateVersion)
        {
            var mappings = processor.GetMappings(templateName, templateVersion);
            return Ok(mappings);
        }

        //{ path: 'templates/:templateName/mappings/:mappingName/versions', component: MappingsTableComponent },
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions")]
        public IActionResult GetTemplateMappingVersions([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var mappings = processor.GetMappings(templateName, mappingName);
            return Ok(mappings);
        }

        //{ path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion', component: MappingsDetailComponent },
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}")]
        public IActionResult GetTemplateMappingInfo([FromRoute]string templateName, [FromRoute]string mappingName, [FromRoute]string mappingVersion)
        {
            var mapping = processor.GetMapping(templateName, mappingName, mappingVersion);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
            //var mapping = repository.GetTemplateMapping(templateName, mappingName, mappingVersion);
            //if (mapping == null)
            //    return NotFound();
            //var processor = new TemplateProcessor();
            //var sources = new List<EvaluationSource>();
            //mapping.Expressions = processor.GetTemplateFieldExpressions(mapping.Buffer, sources);
            //mapping.Sources = sources;
            //return Ok(mapping);
        }



        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        public IActionResult GetTemplateMappingExcel([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var mapping = processor.CreateMapping(templateName, mappingName, $"{Request.Scheme}://{Request.Host}/api/evaluations");
            var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
            return new FileContentResult(mapping.Buffer, contentType)
            {
                FileDownloadName = mapping.FileName
            };
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        public IActionResult CreateTemplateMappingVersion([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var ms = new MemoryStream();
                formFile.CopyTo(ms);
                processor.CreateMapping(templateName, mappingName, ms.ToArray());
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
