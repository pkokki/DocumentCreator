﻿using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentProcessor processor;

        public DocumentsController(IDocumentProcessor processor)
        {
            this.processor = processor;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]DocumentQuery documentParams)
        {
            var documents = processor.GetDocuments(documentParams);
            return Ok(documents);
        }

        [HttpGet]
        [Route("{documentId}")]
        public IActionResult CreateTemplateMappingVersion([FromRoute]string documentId)
        {
            var document = processor.GetDocument(documentId);
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
