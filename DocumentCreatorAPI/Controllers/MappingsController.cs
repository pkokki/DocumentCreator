using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="MappingsController"/> allows you to evaluate standalone Excel formulas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MappingsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingsController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="IMappingProcessor"/> used to process requests.
        /// </param>
        public MappingsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Get the mappings
        /// </summary>
        /// <returns>A list of mappings with relevant information</returns>
        /// <response code="200">Returns a list of mappings with relevant information</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MappingStats>))]
        public IActionResult Get()
        {
            return Ok(processor.GetMappingStats());
        }

        /// <summary>
        /// Get the templates that have a specified mapping
        /// </summary>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>A list of template names</returns>
        /// <response code="200">Returns the list of template names</response>
        [HttpGet]
        [Route("{mappingName}/templates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MappingStats>))]
        public IActionResult GetTemplates([FromRoute]string mappingName)
        {
            var versions = processor.GetMappingStats(mappingName);
            return Ok(versions);
        }
    }
}
