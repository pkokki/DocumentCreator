using DocumentCreator.Core.Model;
using JsonExcelExpressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DocumentCreator.Core
{
    public interface IMappingExpressionEvaluator : IExpressionEvaluator
    {
        EvaluationOutput Evaluate(EvaluationInput input);
    }
}
