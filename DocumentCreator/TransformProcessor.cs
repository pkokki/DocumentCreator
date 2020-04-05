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

        public TransformResult Evaluate(long targetId, string expression, Dictionary<string, JToken> sources)
        {
            string error = null;
            ExcelValue value = null;

            var excelFormula = new ExcelFormula(expression, culture);
            var tokens = new Queue<ExcelFormulaToken>(excelFormula.OfType<ExcelFormulaToken>());

            try
            {
                var scope = new ExcelFormulaValues();
                TraverseScope(scope, tokens, sources);
                var operand = EvaluateSimpleExpression(scope);
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
                Expression = expression,
                Value = value?.ToString(outCulture),
                Error = error
            };
        }

        private void TraverseScope(ExcelFormulaValues scope, Queue<ExcelFormulaToken> tokens, Dictionary<string, JToken> sources)
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
                    var childScope = new ExcelFormulaValues();
                    TraverseScope(childScope, tokens, sources);
                    switch (token.Type)
                    {
                        case ExcelFormulaTokenType.Function:
                            var name = token.Value.ToUpper();
                            var args = EvaluateFunctionArguments(childScope)
                                .Select(o => ExcelValue.Create(o, culture))
                                .ToList();
                            var value = ExcelValue.EvaluateFunction(name, args, culture, sources);
                            scope.Add(new ExcelFormulaValue(value));
                            break;
                        case ExcelFormulaTokenType.Subexpression:
                            var subExp = EvaluateSimpleExpression(childScope);
                            scope.Add(subExp);
                            break;
                        default:
                            throw new NotImplementedException($"Start TokenType={token.Type}");
                    }
                }
                else
                {
                    scope.Add(new ExcelFormulaValue(token, culture));
                }
            }
        }

        private IEnumerable<ExcelFormulaValue> EvaluateFunctionArguments(ExcelFormulaValues scope)
        {
            var args = new List<ExcelFormulaValue>();
            ExcelFormulaValues activeArg = null;
            foreach (var item in scope)
            {
                if (item.HasToken && item.Token.Type == ExcelFormulaTokenType.Argument)
                {
                    args.Add(EvaluateSimpleExpression(activeArg));
                    activeArg = null;
                }
                else
                {
                    activeArg ??= new ExcelFormulaValues();
                    activeArg.Add(item);
                }
            }
            if (activeArg != null) args.Add(EvaluateSimpleExpression(activeArg));
            return args;
        }

        private ExcelFormulaValue EvaluateSimpleExpression(ExcelFormulaValues parts)
        {
            PerformNegation(parts);
            ConvertPercentages(parts);
            PerformExponentiation(parts);
            PerformMultiplicationAndDivision(parts);
            PerformAdditionAndSubtraction(parts);
            EvaluateTextOperators(parts);
            PerformComparisons(parts);

            var value = parts.Single().Value;
            //if (value is string && decimal.TryParse((string)value, NumberStyles.Any, culture, out decimal d))
            //    value = d;
            return new ExcelFormulaValue(value);
        }

        private void PerformComparisons(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Logical);
            while (index > -1)
            {
                var a = parts[index - 1].Value;
                var oper = parts.GetAndRemoveAt(index);
                var b = parts.GetAndRemoveAt(index).Value;
                var result = ExcelValue.CreateBoolean(oper.Token.Value, a, b);
                parts[index - 1] = new ExcelFormulaValue(result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Logical);
            }
        }

        private void PerformAdditionAndSubtraction(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "+", "-");
            while (index > -1)
            {
                var a = ExcelValue.Create(parts[index - 1], culture);
                var oper = parts.GetAndRemoveAt(index);
                var b = ExcelValue.Create(parts.GetAndRemoveAt(index), culture);

                var result = oper.Token.Value == "+" ? a + b : a - b;

                parts[index - 1] = new ExcelFormulaValue(result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "+", "-");
            }
        }

        private void PerformMultiplicationAndDivision(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "*", "/");
            while (index > -1)
            {
                var a = ExcelValue.Create(parts[index - 1], culture);
                var oper = parts.GetAndRemoveAt(index);
                var b = ExcelValue.Create(parts.GetAndRemoveAt(index), culture);
                var result = oper.Token.Value == "*" ? a * b : a / b;
                parts[index - 1] = new ExcelFormulaValue(result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "*", "/");
            }
        }

        private void PerformExponentiation(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "^");
            while (index > -1)
            {
                var a = ExcelValue.Create(parts[index - 1], culture);
                _ = parts.GetAndRemoveAt(index);
                var b = ExcelValue.Create(parts.GetAndRemoveAt(index), culture);
                var result = a ^ b;
                parts[index - 1] = new ExcelFormulaValue(result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "^");
            }
        }
        private void PerformNegation(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorPrefix, "-");
            if (index > -1)
            {
                var operand = parts.GetAndRemoveAt(index + 1);
                parts[index] = new ExcelFormulaValue(-ExcelValue.Create(operand, culture));
            }
        }
        private void ConvertPercentages(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorPostfix, "%");
            if (index > -1)
            {
                parts.RemoveAt(index);
                var operand = parts[index];
                parts[index] = new ExcelFormulaValue(ExcelValue.Create(operand, culture) / 100M);
            }
        }

        private void EvaluateTextOperators(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "&");
            while (index > -1)
            {
                var a = ExcelValue.Create(parts[index - 1], culture);
                _ = parts.GetAndRemoveAt(index);
                var b = ExcelValue.Create(parts.GetAndRemoveAt(index), culture);
                var result = a & b;
                parts[index - 1] = new ExcelFormulaValue(result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "&");
            }
        }

    }
}
