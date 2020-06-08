using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="EvaluationsController"/> allows you to evaluate expressions on JSON sources.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly IMappingProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationsController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="IMappingProcessor"/> used to evaluate expressions.
        /// </param>
        public EvaluationsController(IMappingProcessor processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Εvaluates a collection of template field mappings against some sources.
        /// </summary>
        /// <param name="request">An evaluation request containing the template name, a collection of expressions and a collection of JSON sources</param>
        /// <returns>The output of the evaluation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EvaluationOutput))]
        public IActionResult TestEvaluations([FromBody]EvaluationRequest request)
        {
            var response = processor.Evaluate(request);
            return Ok(response);
        }
    }
}
