using DocumentCreator.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplateDocumentsController : ControllerBase
    {
        private readonly IDocumentProcessor processor;

        public TemplateDocumentsController(IDocumentProcessor processor)
        {
            this.processor = processor;
        }

        //{ path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 2, 3
        //{ path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 1, 2, 3, 4
        //{ path: 'templates/:templateName/versions/:templateVersion/documents', component: DocumentsTableComponent }, // 1, 2



        //{ path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 1, 3, 4
        //{ path: 'templates/:templateName/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 3
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/documents")]
        public IActionResult GetDocuments([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            return Ok($"{templateName}/mappings/{mappingName}/documents");
        }
        //{ path: 'templates/:templateName/documents', component: DocumentsTableComponent }, // 1
        [HttpGet]
        [Route("{templateName}/documents")]
        public IActionResult GetDocuments([FromRoute]string templateName)
        {
            return Ok($"{templateName}/documents");
        }


        [HttpPost]
        [Route("{templateName}/mappings/{mappingName}/document")]
        public IActionResult CreateDocument([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromBody] JObject payload)
        {
            var document = processor.CreateDocument(templateName, mappingName, payload);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return new FileContentResult(document.Buffer, contentType)
            {
                FileDownloadName = document.FileName
            };
        }

    }
}
