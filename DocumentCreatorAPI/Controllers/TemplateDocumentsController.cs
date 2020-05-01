using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;

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

        //{ path: 'templates/:templateName/documents', component: DocumentsTableComponent }, // 1
        //{ path: 'templates/:templateName/versions/:templateVersion/documents', component: DocumentsTableComponent }, // 1, 2
        //{ path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 2, 3
        //{ path: 'templates/:templateName/versions/:templateVersion/mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 1, 2, 3, 4
        //{ path: 'templates/:templateName/mappings/:mappingName/documents', component: DocumentsTableComponent }, // 1, 3
        //{ path: 'templates/:templateName/mappings/:mappingName/versions/:mappingVersion/documents', component: DocumentsTableComponent }, // 1, 3, 4
        [HttpGet]
        [Route("{templateName}/documents")]
        [Route("{templateName}/versions/{templateVersion}/documents")]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/documents")]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions/{mappingVersion}/documents")]
        [Route("{templateName}/mappings/{mappingName}/documents")]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}/documents")]
        public IActionResult GetDocuments([FromRoute]string templateName, 
            [FromRoute]string templateVersion, 
            [FromRoute]string mappingName, 
            [FromRoute]string mappingVersion,
            [FromQuery]PagingParams pagingParams)
        {
            pagingParams ??= new PagingParams();
            var query = new DocumentQuery()
            {
                TemplateName = templateName,
                TemplateVersion = templateVersion,
                MappingsName = mappingName,
                MappingsVersion = mappingVersion,
                Page = pagingParams.Page,
                PageSize = pagingParams.PageSize,
                OrderBy = pagingParams.OrderBy,
                Descending = pagingParams.Descending,
            };
            return Ok(processor.GetDocuments(query));
        }


        [HttpPost]
        [Route("{templateName}/document")]
        [Route("{templateName}/mappings/{mappingName}/document")]
        public IActionResult CreateDocument([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromBody] DocumentPayload payload)
        {
            var document = processor.CreateDocument(templateName, mappingName, payload);
            var fileContents = document.Buffer.ToMemoryStream();
            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return new FileContentResult(fileContents.ToArray(), contentType)
            {
                FileDownloadName = document.FileName
            };
        }

    }
}
