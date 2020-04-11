using DocumentCreator;
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
        public string Get()
        {
            return "Hello, World!";
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
                var testUrl = $"{Request.Scheme}://{Request.Host}/api/templates/{templateName}/mappings/{mappingName}/test";

                var processor = new TemplateProcessor();
                var mappingBuffer = processor.CreateMappingForTemplate(emptyMappingBuffer, mappingName, testUrl, template.Buffer);

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
        [Route("{templateName}/mappings/{mappingName}/{command}")]
        public IActionResult ExecuteMappingCommand([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromRoute]string command,
            [FromBody] JObject payload)
        {
            if ("document".Equals(command, StringComparison.CurrentCultureIgnoreCase))
                return CreateDocument(templateName, mappingName, payload);
            else if ("test".Equals(command, StringComparison.CurrentCultureIgnoreCase))
                return TestMapping(payload);
            else
                return NotFound();
        }

        private IActionResult CreateDocument(string templateName, string mappingName, JObject payload)
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

        private IActionResult TestMapping(JObject payload)
        {
            var sources = new Dictionary<string, JToken>();
            foreach (JToken src in (JArray)payload["sources"])
                sources[src["name"].ToString()] = JObject.Parse(src["value"].ToString());

            var results = new JArray();
            var total = 0;
            var errors = 0;
            var processor = new ExpressionEvaluator();
            foreach (var mapping in (JArray)payload["transformations"])
            {
                var expression = mapping["expression"].ToString();
                var result = processor.Evaluate("F01", expression, sources);

                var json = new JObject
                {
                    ["targetId"] = mapping["targetId"],
                    ["expression"] = mapping["expression"],
                    ["result"] = result.Value
                };
                ++total;
                if (result.Error != null)
                {
                    ++errors;
                    json["error"] = result.Error;
                }
                results.Add(json);
            }
            return Ok(JObject.FromObject(new { total, errors, results }));
        }
    }
}
