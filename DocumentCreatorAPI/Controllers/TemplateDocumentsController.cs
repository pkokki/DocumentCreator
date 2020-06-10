using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="TemplateDocumentsController"/> allows you to manage documents.
    /// </summary>
    [ApiController]
    [Route("api/templates")]
    [SwaggerTag("Create and retrieve documents")]
    public class TemplateDocumentsController : ControllerBase
    {
        private readonly IDocumentProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// An <see cref="IDocumentProcessor"/> used to get or create documents.
        public TemplateDocumentsController(IDocumentProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateDocuments([FromRoute]string templateName,
            [FromQuery]PagingParams pagingParams)
        {
            return GetTemplateVersionMappingVersionDocuments(templateName, null, null, null, pagingParams);
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="templateVersion">An optional filter based on the template version of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateVersionDocuments([FromRoute]string templateName,
            [FromRoute]string templateVersion,
            [FromQuery]PagingParams pagingParams)
        {
            return GetTemplateVersionMappingVersionDocuments(templateName, templateVersion, null, null, pagingParams);
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="templateVersion">An optional filter based on the template version of the document.</param>
        /// <param name="mappingName">An optional filter based on the mapping name of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateVersionMappingDocuments([FromRoute]string templateName,
            [FromRoute]string templateVersion,
            [FromRoute]string mappingName,
            [FromQuery]PagingParams pagingParams)
        {
            return GetTemplateVersionMappingVersionDocuments(templateName, templateVersion, mappingName, null, pagingParams);
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="templateVersion">An optional filter based on the template version of the document.</param>
        /// <param name="mappingName">An optional filter based on the mapping name of the document.</param>
        /// <param name="mappingVersion">An optional filter based on the mapping version of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions/{mappingVersion}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateVersionMappingVersionDocuments([FromRoute]string templateName,
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

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="mappingName">An optional filter based on the mapping name of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateMappingDocuments([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromQuery]PagingParams pagingParams)
        {
            return GetTemplateVersionMappingVersionDocuments(templateName, null, mappingName, null, pagingParams);
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="mappingName">An optional filter based on the mapping name of the document.</param>
        /// <param name="mappingVersion">An optional filter based on the mapping version of the document.</param>
        /// <param name="pagingParams">The pagination information</param>
        /// <returns>A paged list of documents.</returns>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}/documents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        public IActionResult GetTemplateMappingVersionDocuments([FromRoute]string templateName, 
            [FromRoute]string mappingName, 
            [FromRoute]string mappingVersion,
            [FromQuery]PagingParams pagingParams)
        {
            return GetTemplateVersionMappingVersionDocuments(templateName, null, mappingName, mappingVersion, pagingParams);
        }

        /// <summary>
        /// Create and download a document
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="payload">Data to use for the document creation</param>
        /// <returns>The contents of the binary DOCX file</returns>
        /// <response code="200">Returns the contents of the binary DOCX file</response>
        [HttpPost]
        [Route("{templateName}/document")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [Produces("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        public async Task<IActionResult> CreateDocument([FromRoute]string templateName,
            [FromBody] DocumentPayload payload)
        {
            return await CreateDocument(templateName, null, payload);
        }
        
        /// <summary>
        /// Create and download a document
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <param name="payload">Data to use for the document creation</param>
        /// <returns>The contents of the binary DOCX file</returns>
        /// <response code="200">Returns the contents of the binary DOCX file</response>
        [HttpPost]
        [Route("{templateName}/mappings/{mappingName}/document")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [Produces("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        public async Task<IActionResult> CreateDocument([FromRoute]string templateName,
            [FromRoute]string mappingName,
            [FromBody] DocumentPayload payload)
        {
            var document = await processor.CreateDocument(templateName, mappingName, payload);
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
