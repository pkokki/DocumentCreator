using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="DocumentsController"/> allows you to retrieve and create documents.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="IDocumentProcessor"/> used to get or create documents.
        /// </param>
        public DocumentsController(IDocumentProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Get a list of documents.
        /// </summary>
        /// <param name="templateName">An optional filter based on the template name of the document.</param>
        /// <param name="templateVersion">An optional filter based on the template version of the document.</param>
        /// <param name="mappingName">An optional filter based on the mapping name of the document.</param>
        /// <param name="mappingVersion">An optional filter based on the mapping version of the document.</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <param name="orderBy">An optional property name used to order the results.</param>
        /// <param name="descending">The direction of the sort. False for ascending order, true for descending order.</param>
        /// <returns>
        /// A paged list of documents.
        /// </returns>
        /// <remarks>
        /// If more than one filter is submitted, the filters are ANDed.
        /// If no filter is submitted, then all documents are returned.
        /// </remarks>
        /// <response code="200">Returns a paged list of documents.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResults<Document>))]
        [Produces("application/json")]
        public IActionResult Get(
            [FromQuery]string templateName, 
            [FromQuery]string templateVersion, 
            [FromQuery]string mappingName, 
            [FromQuery]string mappingVersion, 
            [FromQuery][DefaultValue(1)] int? page, 
            [FromQuery][DefaultValue(10)] int? pageSize, 
            [FromQuery]string orderBy, 
            [FromQuery][DefaultValue(false)] bool? descending
            )
        {
            var criteria = new DocumentQuery()
            {
                TemplateName = templateName,
                TemplateVersion = templateVersion,
                MappingsName = mappingName,
                MappingsVersion = mappingVersion,
                Page = page ?? 1,
                PageSize = pageSize ?? 10,
                OrderBy = orderBy,
                Descending = descending ?? false,
            };
            var documents = processor.GetDocuments(criteria);
            return Ok(documents);
        }

        /// <summary>
        /// Download the contents of a document.
        /// </summary>
        /// <remarks>
        /// The content-type is set to <code>application/vnd.openxmlformats-officedocument.wordprocessingml.document</code>.
        /// Sets the content-disposition header so that a file-download dialog box is displayed in the browser with a file name.
        /// </remarks>
        /// <param name="documentId">The id of the document</param>
        /// <returns>The contents of the binary DOCX file.</returns>
        /// <response code="200">Returns the contents of the binary DOCX file.</response>
        /// <response code="404">If the document does not exist.</response>
        [HttpGet]
        [Route("{documentId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/json")]
        public IActionResult GetById([FromRoute]string documentId)
        {
            var document = processor.GetDocument(documentId);
            if (document == null || document.Buffer == null)
                return NotFound();
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
