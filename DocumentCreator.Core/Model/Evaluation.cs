using JsonExcelExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    /// <summary>
    /// The evaluation request
    /// </summary>
    public class EvaluationRequest
    {
        /// <summary>
        /// The template name 
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// The collection of expressions to evaluate
        /// </summary>
        public IEnumerable<MappingExpression> Expressions { get; set; }

        /// <summary>
        /// The collection of sources
        /// </summary>
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    public class EvaluationInput
    {
        public IEnumerable<TemplateField> Fields { get; set; }
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    /// <summary>
    /// Input for an expression evaluation
    /// </summary>
    public class ExpressionEvaluationInput
    {
        /// <summary>
        /// A collection of expressions to evaluate.
        /// </summary>
        public IEnumerable<string> Expressions { get; set; }

        /// <summary>
        /// A JSON source for the expressions.
        /// </summary>
        public JObject Payload { get; set; }
    }

    /// <summary>
    /// The output of the evaluation
    /// </summary>
    public class EvaluationOutput
    {
        /// <summary>
        /// Total number of evaluated expressions
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The number of expressions with error
        /// </summary>
        public int Errors { get; set; }

        /// <summary>
        /// The collection of expression evaluation results
        /// </summary>
        public IEnumerable<EvaluationResult> Results { get; set; }
    }

}
