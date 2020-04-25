using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class EvaluationRequest
    {
        public string TemplateName { get; set; }
        public IEnumerable<MappingExpression> Expressions { get; set; }
        public IEnumerable<MappingSource> Sources { get; set; }
    }

    public class Evaluation
    {
        public int Total { get; set; }
        public int Errors { get; set; }

        public IEnumerable<EvaluationResult> Results { get; set; }
    }

    public class EvaluationResult
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
    }
}
