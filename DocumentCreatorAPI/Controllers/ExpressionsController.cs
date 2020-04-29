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
        private readonly IMappingExpressionEvaluator processor;

        public ExpressionsController(IMappingExpressionEvaluator processor)
        {
            this.processor = processor;
        }

        [HttpPost]
        public IActionResult EvaluateExpressions([FromBody]ExpressionEvaluationInput input)
        {
            var response = processor.Evaluate(input.Expressions, input.Payload);
            return Ok(response);
        }
    }
}
