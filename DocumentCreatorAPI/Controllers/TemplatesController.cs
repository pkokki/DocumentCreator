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

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult CreateTemplate()
        {
            var templateName = Request.Form["name"][0];
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var ms = new MemoryStream();
                formFile.CopyTo(ms);
                var contentItem = repository.CreateTemplate(templateName, ms.ToArray());
                
                var templateVersionName = contentItem.Name;
                var conversion = OpenXmlWordConverter.ConvertToHtml(ms, templateVersionName);
                repository.SaveHtml(templateVersionName, null, conversion.Images);

                return Ok(contentItem);
            }
            else
                return BadRequest();
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

        [HttpGet]
        [Route("{templateName}/versions")]
        public IActionResult GetTemplateVersions([FromRoute]string templateName)
        {
            var versions = repository.GetTemplateVersions(templateName);
            return Ok(versions);
        }

        [HttpGet]
        [Route("{templateName}/versions/{version}")]
        public IActionResult GetTemplateVersion([FromRoute]string templateName, [FromRoute]string version)
        {
            var template = repository.GetTemplate(templateName, version);
            var processor = new TemplateProcessor();
            template.Fields = processor.FindTemplateFields(template.Buffer);
            return Ok(template);
        }

        [HttpGet]
        [Route("{templateName}/mappings")]
        public IActionResult GetTemplateMappings([FromRoute]string templateName)
        {
            var mappings = repository.GetTemplateMappings(templateName);
            return Ok(mappings);
        }

        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions")]
        public IActionResult GetTemplateMappingVersions([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            return Ok($"{templateName}/mappings/{mappingName}/versions");
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

        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}")]
        public IActionResult GetTemplateMappingInfo([FromRoute]string templateName, [FromRoute]string mappingName, [FromRoute]string mappingVersion)
        {
            var mapping = repository.GetTemplateMapping(templateName, mappingName, mappingVersion);
            if (mapping == null)
                return NotFound();
            var processor = new TemplateProcessor();
            var sources = new List<EvaluationSource>();
            mapping.Expressions = processor.GetTemplateFieldExpressions(mapping.Buffer, sources);
            mapping.Sources = sources;
            return Ok(mapping);
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

            var templateVersionName = template.Name;
            var conversion = OpenXmlWordConverter.ConvertToHtml(document.Buffer, templateVersionName, document.Name);
            repository.SaveHtml(document.Name, conversion.Html, conversion.Images);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return new FileContentResult(document.Buffer, contentType)
            {
                FileDownloadName = document.FileName
            };
        }

        [HttpGet]
        [Route("{templateName}/documents")]
        public IActionResult GetDocuments([FromRoute]string templateName)
        {
            return Ok($"{templateName}/documents");
        }

        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/documents")]
        public IActionResult GetDocuments([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            return Ok($"{templateName}/mappings/{mappingName}/documents");
        }
    }
}
