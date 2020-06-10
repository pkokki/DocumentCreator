using System;
using System.Collections.Generic;
using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using JsonExcelExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DocumentCreatorAPI.Controllers
{
    /// <summary>
    /// The <see cref="ExpressionsController"/> allows you to evaluate standalone Excel formulas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Evaluate standalone Excel formulas")]
    public class ExpressionsController : ControllerBase
    {
        private readonly IMappingExpressionEvaluator processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsController"/> class.
        /// </summary>
        /// <param name="processor">
        /// An <see cref="IMappingExpressionEvaluator"/> used to evaluate formulas.
        /// </param>
        public ExpressionsController(IMappingExpressionEvaluator processor)
        {
            this.processor = processor;
        }

        /// <summary>
        /// Εvaluates a collection of expressions against an optional JSON payload.
        /// </summary>
        /// <param name="input">The input of the evaluation.</param>
        /// <returns>A list of evaluation results.</returns>
        /// <remarks>
        /// This is an API to evaluate expressions directly without template and mappings.
        /// The expressions are named __A1, __A2, etc and can be referenced in other expressions.
        /// </remarks>
        /// <response code="200">Returns a list of evaluation results</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EvaluationResult>))]
        public IActionResult EvaluateExpressions([FromBody]ExpressionEvaluationInput input)
        {
            var response = processor.Evaluate(input.Expressions, input.Payload);
            return Ok(response);
        }
    }
}
