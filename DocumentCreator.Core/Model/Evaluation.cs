using JsonExcelExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class EvaluationRequest
    {
        public string TemplateName { get; set; }
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    public class EvaluationInput
    {
        public IEnumerable<TemplateField> Fields { get; set; }
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<EvaluationSource> Sources { get; set; }
    }

    public class ExpressionEvaluationInput
    {
        public IEnumerable<string> Expressions { get; set; }
        public JObject Payload { get; set; }
    }

    public class EvaluationOutput
    {
        public int Total { get; set; }
        public int Errors { get; set; }

        public IEnumerable<EvaluationResult> Results { get; set; }
    }

}
