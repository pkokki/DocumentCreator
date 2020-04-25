using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        public EvaluationsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello, Evaluations!";
        }

        [HttpPost]
        public IActionResult TestEvaluations([FromBody]EvaluationRequest request)
        {

            var response = processor.Evaluate(request);
            return Ok(response);
        }
    }
}
