using DocumentCreator.Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly GlobalSettings globalSettings;

        public SettingsController(GlobalSettings globalSettings)
        {
            this.globalSettings = globalSettings;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok(globalSettings);
        }

        [HttpPut]
        [Route("{settingKey}")]
        public IActionResult Set([FromRoute]string settingKey, [FromBody]string value)
        {
            globalSettings.Set(settingKey, value);
            return Ok();
        }
    }
}
