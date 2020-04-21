using DocumentCreator;
using DocumentCreator.Model;
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
    public class MappingsController : ControllerBase
    {
        private readonly IRepository repository;

        public MappingsController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok(repository.GetMappings());
        }

        [HttpGet]
        [Route("/templates/{templateName}")]
        public IActionResult GetForTemplate([FromRoute]string templateName)
        {
            return Ok(repository.GetMappings(templateName));
        }
    }
}
