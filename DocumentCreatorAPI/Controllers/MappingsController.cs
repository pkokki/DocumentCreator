using DocumentCreator.Core;
using DocumentCreator.Core.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MappingsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        public MappingsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok(processor.GetMappingStats());
        }

        [HttpGet]
        [Route("{mappingName}/templates")]
        public IActionResult GetTemplates([FromRoute]string mappingName)
        {
            var versions = processor.GetMappingStats(mappingName);
            return Ok(versions);
        }
    }
}
