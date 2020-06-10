using DocumentCreator.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="SettingsController"/> allows you to get and update settings of the API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly GlobalSettings globalSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsController"/> class.
        /// </summary>
        /// <param name="globalSettings">The global settings of the API</param>
        public SettingsController(GlobalSettings globalSettings)
        {
            this.globalSettings = globalSettings;
        }

        /// <summary>
        /// Get the settings of the API
        /// </summary>
        /// <returns>The settings</returns>
        /// <response code="200">Returns the settings</response>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GlobalSettings))]
        public IActionResult Get()
        {
            return Ok(globalSettings);
        }

        /// <summary>
        /// Update the value of a setting
        /// </summary>
        /// <param name="settingKey">The name of the setting</param>
        /// <param name="value">The new value</param>
        /// <response code="204">If succesfull update</response>
        /// <response code="400">If key or value are invalid</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut]
        [Route("{settingKey}")]
        public IActionResult Set([FromRoute]string settingKey, [FromBody]string value)
        {
            if (string.IsNullOrEmpty(settingKey))
                return BadRequest();
            globalSettings.Set(settingKey, value);
            return NoContent();
        }
    }
}
