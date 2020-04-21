using System;
using System.Collections.Generic;
using System.Linq;
using DocumentCreator.Model;
using DocumentCreator.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IRepository repository;

        public DocumentsController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]DocumentParams documentParams)
        {
            return Ok(repository.GetDocuments(documentParams));
        }
    }
}
