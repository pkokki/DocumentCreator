using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="TemplatesController"/> allows you to manage templates.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Create and retrieve templates")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatesController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="ITemplateProcessor"/> used to manage templates.
        /// </param>
        public TemplatesController(ITemplateProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Get the latest versions of all templates
        /// </summary>
        /// <returns>A collection of templates</returns>
        /// <response code="200">Returns a collection of templates</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Template>))]
        public IActionResult GetTemplates()
        {
            return GetTemplates(null);
        }

        /// <summary>
        /// Get all versions of a template
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <returns>A collection of templates</returns>
        /// <response code="200">Returns a collection of templates</response>
        [HttpGet]
        [Route("{templateName}/versions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Template>))]
        public IActionResult GetTemplates([FromRoute]string templateName)
        {
            var versions = processor.GetTemplates(templateName);
            return Ok(versions);
        }

        /// <summary>
        /// Get details of the latest template version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <returns>Returns the details of the template</returns>
        /// <response code="200">Returns the details of the template</response>
        [HttpGet]
        [Route("{templateName}", Name = "GetTemplate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TemplateDetails))]
        public IActionResult GetTemplate([FromRoute]string templateName)
        {
            return GetTemplate(templateName, null);
        }

        /// <summary>
        /// Get details of a template version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="templateVersion">The template version</param>
        /// <returns>Returns the details of the template version</returns>
        /// <response code="200">Returns the details of the template version</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TemplateDetails))]
        public IActionResult GetTemplate([FromRoute]string templateName, [FromRoute]string templateVersion)
        {
            return Ok(processor.GetTemplate(templateName, templateVersion));
        }


        /// <summary>
        /// Create a new template version
        /// </summary>
        /// <remarks>If the template name exists, a new version is created.</remarks>
        /// <returns>Returns the details of the new template</returns>
        /// <response code="201">Returns the details of the new template</response>
        /// <response code="400">If bad or missing information is provided.</response>
        [HttpPost, DisableRequestSizeLimit]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TemplateDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTemplateAsync([FromForm][BindRequired]string templateName, IFormFile contents)
        {
            if (!string.IsNullOrEmpty(templateName) && contents != null && contents.Length > 0)
            {
                var ms = new MemoryStream();
                contents.CopyTo(ms);
                ms.Position = 0;
                var template = await processor.CreateTemplate(
                    new TemplateData()
                    {
                        TemplateName = templateName
                    }, ms);
                return CreatedAtRoute("GetTemplate", new { templateName = template.TemplateName }, template);
            }
            return BadRequest();
        }
    }
}
