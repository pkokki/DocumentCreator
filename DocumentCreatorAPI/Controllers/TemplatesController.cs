using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DocumentCreator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ILogger<TemplatesController> logger;
        private readonly IWebHostEnvironment hostEnv;
        public TemplatesController(ILogger<TemplatesController> logger, IWebHostEnvironment hostEnv)
        {
            this.logger = logger;
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
        [Route("{templateName}/mappings")]
        public IActionResult GetTemplateEmptyMappings([FromRoute]string templateName)
        {
            var folder = Path.Combine(hostEnv.ContentRootPath, "temp", "templates");
            var templateFiles = Directory.GetFiles(folder, $"{templateName}_*.docx").OrderByDescending(o => o);
            if (templateFiles.Any())
            {
                var processor = new TemplateProcessor();
                var templateBytes = System.IO.File.ReadAllBytes(templateFiles.First());
                var excelBytes = processor.CreateMappingsForTemplate(templateBytes);

                var excelFileName = $"{templateName}.xlsm";
                var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                return new FileContentResult(excelBytes, contentType)
                {
                    FileDownloadName = excelFileName
                };
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("{templateName}/mappings")]
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
    }
}
