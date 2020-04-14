using DocumentCreator;
using DocumentCreator.Model;
using DocumentCreator.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly IRepository repository;

        public TemplatesController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(repository.GetTemplates());
        }

        [HttpGet]
        [Route("{templateName}")]
        public IActionResult GetTemplate([FromRoute]string templateName)
        {
            var template = repository.GetTemplate(templateName);
            var processor = new TemplateProcessor();
            template.Fields = processor.FindTemplateFields(template.Buffer);
            return Ok(template);
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult CreateTemplate()
        {
            var templateName = Request.Form["name"][0];
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var ms = new MemoryStream();
                formFile.CopyTo(ms);
                repository.CreateTemplate(templateName, ms.ToArray());
                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}")]
        public IActionResult GetTemplateMapping([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var mapping = repository.GetLatestMapping(templateName, mappingName);
            if (mapping == null)
            {
                var template = repository.GetLatestTemplate(templateName);
                if (template == null)
                    return NotFound();
                var emptyMappingBuffer = repository.GetEmptyMapping().Buffer;
                var testUrl = $"{Request.Scheme}://{Request.Host}/api/evaluations";

                var processor = new TemplateProcessor();
                var mappingBuffer = processor.CreateMappingForTemplate(emptyMappingBuffer, templateName, mappingName, testUrl, template.Buffer);

                mapping = repository.CreateMapping(templateName, mappingName, mappingBuffer);
            }
            var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
            return new FileContentResult(mapping.Buffer, contentType)
            {
                FileDownloadName = mapping.FileName
            };
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("{templateName}/mappings/{mappingName}")]
        public IActionResult CreateTemplateMapping([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var ms = new MemoryStream();
                formFile.CopyTo(ms);
                repository.CreateMapping(templateName, mappingName, ms.ToArray());
                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("{templateName}/mappings/{mappingName}/document")]
        public IActionResult CreateDocument([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromBody] JObject payload)
        {
            var processor = new TemplateProcessor();
            var template = repository.GetLatestTemplate(templateName);
            var mapping = repository.GetLatestMapping(templateName, mappingName);

            var documentBytes = processor.CreateDocument(template.Buffer, mapping.Buffer, payload);
            var document = repository.CreateDocument(templateName, mappingName, documentBytes);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return new FileContentResult(document.Buffer, contentType)
            {
                FileDownloadName = document.FileName
            };
        }
    }
}
