using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DocumentCreator
{
    public class TransformProcessor
    {
        private readonly CultureInfo culture, outCulture;

        public TransformProcessor() : this(CultureInfo.InvariantCulture, CultureInfo.CurrentCulture)
        {
        }

        public TransformProcessor(CultureInfo culture, CultureInfo outCulture)
        {
            this.culture = culture;
            this.outCulture = outCulture;
        }

        public TransformResponse Test(JObject request)
        {
            var response = new TransformResponse(request["name"].ToString());

            var sources = new Dictionary<string, JToken>();
            foreach (var source in request["sources"] as JArray)
            {
                sources.Add(source["name"].ToString(), JObject.Parse(source["value"].ToString()));
            }
            var transformations = request["transformations"] as JArray;
            foreach (var transformation in transformations)
            {
                var targetId = (long)transformation["targetId"];
                var expression = (string)transformation["expression"];
                //var expected = (string)transformation["expected"];
                //var comment = (string)transformation["comment"];

                var result = Evaluate(targetId, expression, sources);
                response.AddResult(result);
            }
            return response;
        }

        public TransformResult Evaluate(long targetId, string formula, Dictionary<string, JToken> sources)
        {
            string error = null;
            ExcelValue value = null;

            var excelFormula = new ExcelFormula(formula, culture);
            var tokens = new Queue<ExcelFormulaToken>(excelFormula.OfType<ExcelFormulaToken>());

            try
            {
                var expression = new ExcelExpression();
                TraverseExpression(expression, tokens, sources);
                var operand = expression.Evaluate();
                value = operand.Value;
                if (value.InnerValue is JArray)
                    sources["#ROW#"] = ((JArray)value.InnerValue).FirstOrDefault();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return new TransformResult()
            {
                TargetId = targetId,
                Expression = formula,
                Value = value?.ToString(outCulture),
                Error = error
            };
        }

        private void TraverseExpression(ExcelExpression expression, Queue<ExcelFormulaToken> tokens, Dictionary<string, JToken> sources)
        {
            while (tokens.Any())
            {
                var token = tokens.Dequeue();
                if (token.Subtype == ExcelFormulaTokenSubtype.Stop)
                {
                    return;
                }
                else if (token.Subtype == ExcelFormulaTokenSubtype.Start)
                {
                    var childExpression = new ExcelExpression();
                    TraverseExpression(childExpression, tokens, sources);
                    switch (token.Type)
                    {
                        case ExcelFormulaTokenType.Function:
                            var name = token.Value.ToUpper();
                            var args = EvaluateFunctionArguments(childExpression)
                                .Select(o => o.Value)
                                .ToList();
                            var value = ExcelValue.EvaluateFunction(name, args, culture, sources);
                            expression.Add(new ExcelExpressionPart(value));
                            break;
                        case ExcelFormulaTokenType.Subexpression:
                            var subExpression = childExpression.Evaluate();
                            expression.Add(subExpression);
                            break;
                        default:
                            throw new NotImplementedException($"Start TokenType={token.Type}");
                    }
                }
                else
                {
                    expression.Add(new ExcelExpressionPart(token, culture));
                }
            }
        }

        private IEnumerable<ExcelExpressionPart> EvaluateFunctionArguments(ExcelExpression expression)
        {
            var args = new List<ExcelExpressionPart>();
            ExcelExpression activeArg = null;
            foreach (var item in expression)
            {
                if (item.TokenType == ExcelFormulaTokenType.Argument)
                {
                    args.Add(activeArg.Evaluate());
                    activeArg = null;
                }
                else
                {
                    activeArg ??= new ExcelExpression();
                    activeArg.Add(item);
                }
            }
            if (activeArg != null) args.Add(activeArg.Evaluate());
            return args;
        }
    }
}
