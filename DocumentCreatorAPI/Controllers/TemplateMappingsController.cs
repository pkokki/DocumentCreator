using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
        //{ path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/versions', component: MappingsTableComponent },
        //{ path: 'templates/:templateName/mappings/:mappingName/versions', component: MappingsTableComponent },
        //{ path: 'templates/:templateName/versions/:templateVersion/mappings', component: MappingsTableComponent },
        [HttpGet]
        [Route("{templateName}/mappings")]
        [Route("{templateName}/versions/{templateVersion}/mappings")]
        [Route("{templateName}/mappings/{mappingName}/versions")]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions")]
        public IActionResult GetTemplateMappingVersions([FromRoute]string templateName, [FromRoute]string templateVersion, [FromRoute]string mappingName)
        {
            var mappings = processor.GetMappings(templateName, templateVersion, mappingName);
            return Ok(mappings);
        }

        //{ path: 'templates/:templateName/mappings/:mappingName', component: MappingsDetailComponent },
        //{ path: 'templates/:templateName/versions/:versionName/mappings/:mappingName', component: MappingsDetailComponent },
        //{ path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion', component: MappingsDetailComponent },
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}")]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}")]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}")]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions/{mappingVersion}")]
        public IActionResult GetTemplateMappingVersionInfo([FromRoute]string templateName, [FromRoute]string templateVersion, [FromRoute]string mappingName, [FromRoute]string mappingVersion)
        {
            var mapping = processor.GetMapping(templateName, templateVersion, mappingName, mappingVersion);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
        }



        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        public async Task<IActionResult> GetTemplateMappingExcel([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            return await GetTemplateMappingExcelWithSources(templateName, mappingName, null);
        }

        [HttpPut]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        public async Task<IActionResult> GetTemplateMappingExcelWithSources(
            [FromRoute]string templateName, 
            [FromRoute]string mappingName,
            [FromBody]FillMappingPayload payload)
        {
            var info = new FillMappingInfo()
            {
                TemplateName = templateName,
                MappingName = mappingName,
                TestUrl = $"{Request.Scheme}://{Request.Host}/api/evaluations",
                Payload = payload
            };

            var mapping = await processor.BuildMapping(info);

            var fileContents = mapping.Buffer.ToMemoryStream();
            var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
            return new FileContentResult(fileContents.ToArray(), contentType)
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
                ms.Position = 0;
                processor.CreateMapping(templateName, mappingName, ms);
                return Ok();
            }
            else
                return BadRequest();
        }
    }
}
