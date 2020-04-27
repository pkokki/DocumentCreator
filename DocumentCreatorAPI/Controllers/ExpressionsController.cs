using System;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace DocumentCreatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpressionsController : ControllerBase
    {
        private readonly IExpressionEvaluator processor;

        public ExpressionsController(IExpressionEvaluator processor)
        {
            this.processor = processor;
        }

        [HttpPost]
        public IActionResult EvaluateExpressions([FromBody]ExpressionEvaluationInput input)
        {
            var response = processor.Evaluate(input);
            return Ok(response);
        }
    }
}
