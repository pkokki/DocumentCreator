using DocumentCreator.Core.Repository;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get([FromQuery]string templateName)
        {
            return Ok(repository.GetMappingStats(templateName));
        }
    }
}
