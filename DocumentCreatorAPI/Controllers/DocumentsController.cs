using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(processor.GetDocuments(documentParams));
        }
    }
}
