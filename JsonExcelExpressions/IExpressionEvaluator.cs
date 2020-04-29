using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace JsonExcelExpressions
{
    public interface IExpressionEvaluator
    {
        EvaluationResult Evaluate(string expression);
        EvaluationResult Evaluate(string expression, JObject source);
        IEnumerable<EvaluationResult> Evaluate(IEnumerable<string> expressions, JObject source);
    }
}
