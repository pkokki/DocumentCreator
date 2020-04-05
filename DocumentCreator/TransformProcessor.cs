using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.Model;
using Newtonsoft.Json.Linq;

namespace DocumentCreator
{
    public class TransformProcessor
    {
        private static readonly SpecialResult DIV0 = new SpecialResult("#DIV/0!");
        private static readonly SpecialResult NA = new SpecialResult("#N/A");

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
            object value = null;

            var excelFormula = new ExcelFormula(expression, culture);
            var tokens = new Queue<ExcelFormulaToken>(excelFormula.OfType<ExcelFormulaToken>());

            try
            {
                var parts = new ExcelFormulaValues();
                TraverseScope(parts, tokens, sources);
                var operand = EvaluateSimpleExpression(parts);
                value = operand.Value;
                if (value is JArray)
                    sources["#ROW#"] = ((JArray)value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return new TransformResult()
            {
                TargetId = targetId,
                Expression = expression,
                Value = ConvertToString(value),
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
                            var args = EvaluateFunctionArguments(childScope);
                            var value = EvaluateFunction(token.Value.ToUpper(), args, sources);
                            scope.Add(new ExcelFormulaValue(null, value));
                            break;
                        case ExcelFormulaTokenType.Subexpression:
                            var subExp = EvaluateSimpleExpression(childScope);
                            scope.Add(subExp);
                            break;
                        default:
                            throw new NotImplementedException($"TokenType={token.Type}");
                    }
                }
                else
                {
                    scope.Add(new ExcelFormulaValue(token));
                }
            }
        }

        private object[] EvaluateFunctionArguments(ExcelFormulaValues scope)
        {
            var args = new List<object>();
            ExcelFormulaValues activeArg = null;
            foreach(var item in scope)
            {
                if (item.HasToken && item.Token.Type == ExcelFormulaTokenType.Argument)
                {
                    args.Add(EvaluateSimpleExpression(activeArg).Value);
                    activeArg = null;
                }
                else
                {
                    activeArg ??= new ExcelFormulaValues();
                    activeArg.Add(item);
                }
            }
            if (activeArg != null) args.Add(EvaluateSimpleExpression(activeArg).Value);
            return args.ToArray();
        }

        
        private object EvaluateFunction(string name, object[] args, Dictionary<string, JToken> sources)
        {
            switch (name)
            {
                // EXCEL FUNCTIONS
                case "LEN":
                    return Convert.ToString(args[0], culture).Length;
                case "UPPER":
                    return args[0] == null ? null : Convert.ToString(args[0], culture).ToUpper();
                case "LOWER":
                    return args[0] == null ? null : Convert.ToString(args[0], culture).ToLower();
                case "PROPER":
                    return args[0] == null ? null : culture.TextInfo.ToTitleCase(Convert.ToString(args[0], culture));
                // EXCEL LOGICAL FUNCTIONS
                case "IF":
                    return Convert.ToBoolean(args[0]) ? args[1] : args[2];
                case "AND":
                    return args.All(o => Convert.ToBoolean(o));
                case "OR":
                    return args.Any(o => Convert.ToBoolean(o));
                case "XOR":
                    return args.Aggregate((a, b) => Convert.ToBoolean(a) ^ Convert.ToBoolean(b));
                case "NOT":
                    return !Convert.ToBoolean(args[0]);
                case "IFERROR":
                    return args[0] is SpecialResult ? args[1] : args[0];
                case "IFNA":
                    return NA.Equals(args[0]) ? args[1] : args[0];

                case "NA":
                    return NA;
                // CUSTOM UDF FUNCTIONS
                case "SYSDATE":
                    return DateTime.Today.ToString("d/M/yyyy");
                case "SOURCE":
                    return sources[args[0].ToString()]?.SelectToken(args[1].ToString());
                case "RQD":
                    return sources["RQ"]?["RequestData"]?[args[0]];
                case "RQL":
                    return sources["RQ"]?["LogHeader"]?[args[0]];
                case "RQR":
                    return sources["#ROW#"]?[args[0]];
                case "CONTENT":
                    return Convert.ToBoolean(args[0]) ? "#EMPTY_CONTENT#" : null;
                default:
                    throw new InvalidOperationException($"Unknown function name: {name}");
            }
        }

        private ExcelFormulaValue EvaluateSimpleExpression(ExcelFormulaValues parts)
        {
            // Evaluate items in parentheses
            PerformNegation(parts);
            ConvertPercentages(parts);
            // Perform exponentiation (^)
            PerformMultiplicationAndDivision(parts);
            PerformAdditionAndSubtraction(parts);
            // Evaluate text operators (&)
            PerformComparisons(parts);

            var value = parts.Single().Value;
            if (value is string && decimal.TryParse((string)value, NumberStyles.Any, culture, out decimal d))
                value = d;
            return new ExcelFormulaValue(null, value);
        }

        private void PerformComparisons(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Logical);
            while (index > -1)
            {
                var a = parts[index - 1].Value;
                var oper = parts.GetAndRemoveAt(index);
                var b = parts.GetAndRemoveAt(index).Value;
                var result = EvaluateLogical(oper.Token.Value, a, b);
                parts[index - 1] = new ExcelFormulaValue(null, result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, ExcelFormulaTokenSubtype.Logical);
            }
        }

        private object EvaluateLogical(string oper, object a, object b)
        {
            if (a == null)
                return b == null;
            else if (b == null)
                return false;
            // Both not null
            object a1, b1;
            try
            {
                a1 = a;
                b1 = Convert.ChangeType(b, a.GetType());
            }
            catch
            {
                b1 = b;
                try
                {
                    a1 = Convert.ChangeType(a, b.GetType());
                }
                catch
                {
                    return NA;
                }
            }
            var comparable = a1 as IComparable;
            if (comparable == null)
                return NA;

            switch (oper)
            {
                case "=": return comparable.CompareTo(b1) == 0;
                case ">": return comparable.CompareTo(b1) > 0;
                case ">=": return comparable.CompareTo(b1) >= 0;
                case "<": return comparable.CompareTo(b1) < 0;
                case "<=": return comparable.CompareTo(b1) <= 0;
                default: throw new InvalidOperationException($"Unknown logical operator: {oper}");
            }
        }

        private void PerformAdditionAndSubtraction(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "+", "-");
            while (index > -1)
            {
                var a = Convert.ToDecimal(parts[index - 1].Value, culture);
                var oper = parts.GetAndRemoveAt(index);
                var b = Convert.ToDecimal(parts.GetAndRemoveAt(index).Value, culture);
                var result = oper.Token.Value == "+" ? a + b : a - b;
                parts[index - 1] = new ExcelFormulaValue(null, result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "+", "-");
            }
        }

        private void PerformMultiplicationAndDivision(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "*", "/");
            while (index > -1)
            {
                var a = Convert.ToDecimal(parts[index - 1].Value, culture);
                var oper = parts.GetAndRemoveAt(index);
                var b = Convert.ToDecimal(parts.GetAndRemoveAt(index).Value, culture);
                var result = oper.Token.Value == "*" ? PerformMultiplication(a, b) : PerformDivision(a, b);
                parts[index - 1] = new ExcelFormulaValue(null, result);
                index = parts.IndexOf(ExcelFormulaTokenType.OperatorInfix, "*", "/");
            }
        }

        private object PerformMultiplication(decimal a, decimal b)
        {
            return a * b;
        }
        private object PerformDivision(decimal a, decimal b)
        {
            if (b == 0)
                return DIV0;
            return a / b;
        }

        private void PerformNegation(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorPrefix, "-");
            if (index > -1)
            {
                var operand = parts.GetAndRemoveAt(index + 1);
                parts[index] = new ExcelFormulaValue(null, -Convert.ToDecimal(operand.Value, culture));
            }
        }
        private void ConvertPercentages(ExcelFormulaValues parts)
        {
            var index = parts.IndexOf(ExcelFormulaTokenType.OperatorPostfix, "%");
            if (index > -1)
            {
                parts.RemoveAt(index);
                var operand = parts[index];
                parts[index] = new ExcelFormulaValue(null, Convert.ToDecimal(operand.Value) / 100);
            }
        }

        private string ConvertToString(object value)
        {
            if (value == null)
                return string.Empty;
            else if (value is JArray)
                return "[]";
            else if (value is JObject)
                return "{}";
            else if (value is bool)
                return Convert.ToString(value).ToUpper();
            return Convert.ToString(value, outCulture);
        }

        public class SpecialResult
        {
            private readonly string value;

            public SpecialResult(string value)
            {
                this.value = value;
            }
            public override string ToString()
            {
                return value;
            }
        }
    }
}
