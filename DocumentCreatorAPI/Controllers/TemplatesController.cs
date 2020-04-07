﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentCreator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnv;
        public TemplatesController(IWebHostEnvironment hostEnv)
        {
            this.hostEnv = hostEnv;
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
                var folder = Path.Combine(hostEnv.ContentRootPath, "temp", "templates");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var fileName = $"{templateName}_{DateTime.Now.Ticks}.docx";
                var fullFileName = Path.Combine(folder, fileName);
                using var stream = new FileStream(fullFileName, FileMode.Create);
                formFile.CopyTo(stream);
               return Ok();
            }
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("{templateName}/mappings/{mappingsName}")]
        public IActionResult GetTemplateMappings([FromRoute]string templateName, [FromRoute]string mappingsName)
        {
            var templatesFolder = Path.Combine(hostEnv.ContentRootPath, "temp", "templates");
            var latestTemplateFileName = Directory
                .GetFiles(templatesFolder, $"{templateName}_*.docx")
                .OrderByDescending(o => o)
                .FirstOrDefault();
            if (latestTemplateFileName != null)
            {
                var mappingsFolder = Path.Combine(hostEnv.ContentRootPath, "temp", "mappings");
                var latestMappingsFileName = Directory
                    .GetFiles(mappingsFolder, $"{Path.GetFileNameWithoutExtension(latestTemplateFileName)}_{mappingsName}_*.xlsm")
                    .OrderByDescending(o => o)
                    .FirstOrDefault();
                
                byte[] mappingsBuffer;
                string mappingsFileName;
                if (latestMappingsFileName != null)
                {
                    mappingsBuffer = System.IO.File.ReadAllBytes(latestMappingsFileName);
                    mappingsFileName = Path.GetFileName(latestMappingsFileName);
                }
                else
                {
                    var emptyMappingsPath = Path.Combine(hostEnv.ContentRootPath, "resources", "empty_mappings.xlsm");
                    var templateBytes = System.IO.File.ReadAllBytes(latestTemplateFileName);

                    var processor = new TemplateProcessor();
                    var testUrl = $"{Request.Scheme}://{Request.Host}/api/templates/{templateName}/mappings/{mappingsName}/test";
                    mappingsBuffer = processor.CreateMappingsForTemplate(emptyMappingsPath, mappingsName, testUrl, templateBytes);
                    mappingsFileName = $"{Path.GetFileNameWithoutExtension(latestTemplateFileName)}_{mappingsName}_{DateTime.Now.Ticks}.xlsm";

                    System.IO.File.WriteAllBytes(Path.Combine(mappingsFolder, mappingsFileName), mappingsBuffer);
                }
                var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                return new FileContentResult(mappingsBuffer, contentType)
                {
                    FileDownloadName = mappingsFileName
                };
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("{templateName}/mappings/{mappingsName}")]
        public IActionResult CreateTemplateMappings([FromRoute]string templateName, [FromRoute]string mappingsName)
        {
            var formFile = Request.Form.Files[0];
            if (formFile.Length > 0)
            {
                var folder = Path.Combine(hostEnv.ContentRootPath, "temp", "mappings");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var fileName = $"{templateName}_{mappingsName}_{DateTime.Now.Ticks}.xlsm";
                var fullFileName = Path.Combine(folder, fileName);
                using var stream = new FileStream(fullFileName, FileMode.Create);
                formFile.CopyTo(stream);
                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpPost]
        [Route("{templateName}/mappings/{mappingsName}/document")]
        public IActionResult CreateDocument([FromRoute]string templateName, [FromRoute]string mappingsName, [FromBody] JObject payload)
        {
            var folder = Path.Combine(hostEnv.ContentRootPath, "temp", "mappings");
            var mappingFiles = Directory.GetFiles(folder, $"{templateName}_{mappingsName}_*.xlsm").OrderByDescending(o => o);
            if (mappingFiles.Any())
            {
                var mappingInfo = new FileInfo(mappingFiles.First());
                var mappingBytes = System.IO.File.ReadAllBytes(mappingInfo.FullName);

                var processor = new TemplateProcessor();
                var documentBytes = processor.CreateDocument(mappingBytes, payload);

                var documentFileName = $"{mappingInfo.Name}_{DateTime.Now.Ticks}.docx";
                System.IO.File.WriteAllBytes(Path.Combine(hostEnv.ContentRootPath, "temp", "documents", documentFileName), documentBytes);

                var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                return new FileContentResult(documentBytes, contentType)
                {
                    FileDownloadName = documentFileName
                };
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("{templateName}/mappings/{mappingsName}/test")]
        public IActionResult TestMappings([FromRoute]string templateName, [FromRoute]string mappingsName, [FromBody] JObject payload)
        {
            var processor = new TransformProcessor(CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("el-GR"));

            var sources = new Dictionary<string, JToken>();
            foreach (JObject src in (JArray)payload["sources"])
                sources[src["name"].ToString()] = src["value"];

            var results = new JArray();
            var total = 0;
            var errors = 0;
            foreach (var mapping in (JArray)payload["transformations"])
            {
                var expression = mapping["expression"].ToString();
                var result = processor.Evaluate(0, expression, null);

                var json = new JObject();
                json["targetId"] = mapping["targetId"];
                json["expression"] = mapping["expression"];
                json["result"] = result.Value;
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
