using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DocumentCreator.Core
{
    public interface IExpressionEvaluator
    {
        EvaluationOutput Evaluate(EvaluationInput input);
        IEnumerable<EvaluationResult> Evaluate(ExpressionEvaluationInput input);
        EvaluationResult Evaluate(string expression, JObject payload);
    }
}
