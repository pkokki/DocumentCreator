using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace JsonExcelExpressions
{
    public interface IExpressionEvaluator
    {
        EvaluationResult Evaluate(string expression, CultureInfo culture = null);
        EvaluationResult Evaluate(string expression, JObject source, CultureInfo culture = null);
        IEnumerable<EvaluationResult> Evaluate(IEnumerable<string> expressions, JObject source, CultureInfo culture = null);
    }
}
