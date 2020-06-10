using DocumentCreator;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="TemplateMappingsController"/> allows you to manage template mappings.
    /// </summary>
    [ApiController]
    [Route("api/templates")]
    [SwaggerTag("Create and retrieve template mappings")]
    public class TemplateMappingsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMappingsController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="IMappingProcessor"/> used to manage template mappings.
        /// </param>
        public TemplateMappingsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Get a collection of mappings for the latest version of a template
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <returns>A collection of mappings</returns>
        /// <response code="200">Returns a collection of mappings</response>
        [HttpGet]
        [Route("{templateName}/mappings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Mapping>))]
        public IActionResult GetTemplateMappings([FromRoute]string templateName)
        {
            var mappings = processor.GetMappings(templateName, null, null);
            return Ok(mappings);
        }

        /// <summary>
        /// Get a collection of mappings for the specified version of a template
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="templateVersion">The template version</param>
        /// <returns>A collection of mappings</returns>
        /// <response code="200">Returns a collection of mappings</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Mapping>))]
        public IActionResult GetTemplateVersionMappings([FromRoute]string templateName, [FromRoute]string templateVersion)
        {
            var mappings = processor.GetMappings(templateName, templateVersion, null);
            return Ok(mappings);
        }

        /// <summary>
        /// Get a collection of mapping versions for the latest version of a template and for the specified mapping
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>A collection of mappings</returns>
        /// <response code="200">Returns a collection of mappings</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Mapping>))]
        public IActionResult GetTemplateMappingVersions([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var mappings = processor.GetMappings(templateName, null, mappingName);
            return Ok(mappings);
        }

        /// <summary>
        /// Get a collection of mapping versions for the specified version of a template and for the specified mapping
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="templateVersion">The template version</param>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>A collection of mappings</returns>
        /// <response code="200">Returns a collection of mappings</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Mapping>))]
        public IActionResult GetTemplateMappingVersions([FromRoute]string templateName, [FromRoute]string templateVersion, [FromRoute]string mappingName)
        {
            var mappings = processor.GetMappings(templateName, templateVersion, mappingName);
            return Ok(mappings);
        }

        /// <summary>
        /// Get the mapping details for the latest template version and the latest mapping version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>The details of the mapping version</returns>
        /// <response code="200">Returns the details of the mapping version</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MappingDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTemplateMappingInfo([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            var mapping = processor.GetMapping(templateName, null, mappingName, null);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
        }

        /// <summary>
        /// Get the mapping details for the latest template version and the specified mapping version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <param name="mappingVersion">The mapping version</param>
        /// <returns>The details of the mapping version</returns>
        /// <response code="200">Returns the details of the mapping version</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/versions/{mappingVersion}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MappingDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTemplateMappingVersionInfo([FromRoute]string templateName, [FromRoute]string mappingName, [FromRoute]string mappingVersion)
        {
            var mapping = processor.GetMapping(templateName, null, mappingName, mappingVersion);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
        }

        /// <summary>
        /// Get the mapping details for the specified template version and the latest mapping version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="templateVersion">The template version</param>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>The details of the mapping version</returns>
        /// <response code="200">Returns the details of the mapping version</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MappingDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTemplateVersionMappingInfo([FromRoute]string templateName, [FromRoute]string templateVersion, [FromRoute]string mappingName)
        {
            var mapping = processor.GetMapping(templateName, templateVersion, mappingName, null);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
        }

        /// <summary>
        /// Get the mapping details for the specified template version and the specified mapping version
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="templateVersion">The template version</param>
        /// <param name="mappingName">The mapping name</param>
        /// <param name="mappingVersion">The mapping version</param>
        /// <returns>The details of the mapping version</returns>
        /// <response code="200">Returns the details of the mapping version</response>
        [HttpGet]
        [Route("{templateName}/versions/{templateVersion}/mappings/{mappingName}/versions/{mappingVersion}", Name = "GetMappingVersion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MappingDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTemplateVersionMappingVersionInfo([FromRoute]string templateName, [FromRoute]string templateVersion, [FromRoute]string mappingName, [FromRoute]string mappingVersion)
        {
            var mapping = processor.GetMapping(templateName, templateVersion, mappingName, mappingVersion);
            if (mapping == null)
                return NotFound();
            return Ok(mapping);
        }


        /// <summary>
        /// Downloads the content of a mapping Excel file
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <returns>The contents of the binary XLSX file.</returns>
        /// <response code="200">Returns the contents of the binary XLSX file.</response>
        /// <response code="404">If the document does not exist.</response>
        [HttpGet]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.ms-excel.sheet.macroEnabled.12")]
        public async Task<IActionResult> GetTemplateMappingExcel([FromRoute]string templateName, [FromRoute]string mappingName)
        {
            return await GetTemplateMappingExcelWithSources(templateName, mappingName, null);
        }

        /// <summary>
        /// Downloads the content of a mapping Excel file
        /// </summary>
        /// <param name="templateName">The template name</param>
        /// <param name="mappingName">The mapping name</param>
        /// <param name="payload"></param>
        /// <returns>The contents of the binary XLSX file.</returns>
        /// <response code="200">Returns the contents of the binary XLSX file.</response>
        /// <response code="404">If the document does not exist.</response>
        [HttpPut]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.ms-excel.sheet.macroEnabled.12")]
        public async Task<IActionResult> GetTemplateMappingExcelWithSources(
            [FromRoute]string templateName, 
            [FromRoute]string mappingName,
            [FromBody]FillMappingPayload payload)
        {
            var info = new FillMappingInfo()
            {
                TemplateName = templateName,
                MappingName = mappingName,
                TestUrl = $"{Request.Scheme}://{Request.Host}/api/evaluations",
                Payload = payload
            };

            var mapping = await processor.BuildMapping(info);

            var fileContents = mapping.Buffer.ToMemoryStream();
            var contentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
            return new FileContentResult(fileContents.ToArray(), contentType)
            {
                FileDownloadName = mapping.FileName
            };
        }

        /// <summary>
        /// Create a new mapping version
        /// </summary>
        /// <remarks>If the mapping name exists, a new version is created.</remarks>
        /// <returns>Returns the details of the new mapping</returns>
        /// <response code="201">Returns the details of the new mapping</response>
        /// <response code="400">If bad or missing information is provided.</response>
        [HttpPost, DisableRequestSizeLimit]
        [Route("{templateName}/mappings/{mappingName}/xls")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MappingDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTemplateMappingVersion([FromRoute]string templateName, [FromRoute]string mappingName, IFormFile contents)
        {
            if (contents != null && contents.Length > 0)
            {
                var ms = new MemoryStream();
                contents.CopyTo(ms);
                ms.Position = 0;
                var mapping = await processor.CreateMapping(templateName, mappingName, ms);
                return CreatedAtRoute("GetMappingVersion", new { 
                    templateName = mapping.TemplateName, 
                    templateVersion = mapping.TemplateVersion,
                    mappingName = mapping.MappingName,
                    mappingVersion = mapping.MappingVersion
                }, mapping);
            }
            else
                return BadRequest();
        }
    }
}
