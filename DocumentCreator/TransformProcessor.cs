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

        private TransformResult Evaluate(long targetId, string expression, Dictionary<string, JToken> sources)
        {
            string error = null;
            object value = null;

            var excelFormula = new ExcelFormula(expression);
            var tokens = new Queue<ExcelFormulaToken>(excelFormula.OfType<ExcelFormulaToken>());

            try
            {
                value = Evaluate(tokens, sources);
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

        private string ConvertToString(object value)
        {
            if (value is JArray)
                return "[]";
            else if (value is JObject)
                return "{}";
            return value?.ToString();
        }

        private object Evaluate(Queue<ExcelFormulaToken> tokens, Dictionary<string, JToken> sources)
        {
            object result;
            var nextToken = tokens.Peek();
            switch (nextToken.Type)
            {
                case ExcelFormulaTokenType.Function:
                    result = EvaluateFunction(tokens, sources);
                    break;
                case ExcelFormulaTokenType.Operand:
                    result = EvaluateOperand(tokens, sources);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown next token: {nextToken.Type}");
            }
            if (tokens.Any())
            {
                nextToken = tokens.Peek();
                if (nextToken.Type == ExcelFormulaTokenType.OperatorInfix)
                {
                    nextToken = tokens.Dequeue();
                    switch (nextToken.Subtype)
                    {
                        case ExcelFormulaTokenSubtype.Math:
                            result = EvaluateMath(nextToken.Value, result, Evaluate(tokens, sources));
                            break;
                        case ExcelFormulaTokenSubtype.Logical:
                            result = EvaluateLogical(nextToken.Value, result, Evaluate(tokens, sources));
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown OperatorInfix subType: {nextToken.Subtype}");
                    }
                }
            }
            return result;
        }

        private bool EvaluateLogical(string oper, object left, object right)
        {
            if (left == null)
                return right == null;

            var comparable = left as IComparable;
            var other = right;
            if (comparable == null)
            {
                comparable = right as IComparable;
                other = left;
            }
            if (comparable == null)
                throw new InvalidOperationException($"Cannot evaluate logical expression between {left} and {right}");

            switch (oper)
            {
                case "=": return comparable.CompareTo(other) == 0;
                case ">": return comparable.CompareTo(other) > 0;
                case ">=": return comparable.CompareTo(other) >= 0;
                case "<": return comparable.CompareTo(other) < 0;
                case "<=": return comparable.CompareTo(other) <= 0;
                default: throw new InvalidOperationException($"Unknown logical operator: {oper}");
            }
        }

        private object EvaluateMath(string oper, object left, object right)
        {
            switch (oper)
            {
                case "+": return Convert.ToDouble(left) + Convert.ToDouble(right);
                case "-": return Convert.ToDouble(left) - Convert.ToDouble(right);
                case "*": return Convert.ToDouble(left) * Convert.ToDouble(right);
                case "/": return Convert.ToDouble(left) / Convert.ToDouble(right);
                default: throw new InvalidOperationException($"Unknown math operator: {oper}");
            }
        }

        private object EvaluateFunction(Queue<ExcelFormulaToken> tokens, Dictionary<string, JToken> sources)
        {
            var token = tokens.Dequeue();
            EnsureToken("FunctionStart", token, ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Start);

            var name = token.Value.ToUpper();
            var arguments = new List<object>();

            token = tokens.Peek();
            while (!(token.Type == ExcelFormulaTokenType.Function && token.Subtype == ExcelFormulaTokenSubtype.Stop))
            {
                arguments.Add(Evaluate(tokens, sources));
                token = tokens.Peek();
                if (token.Type == ExcelFormulaTokenType.Argument && token.Subtype == ExcelFormulaTokenSubtype.Nothing)
                {
                    tokens.Dequeue();
                    token = tokens.Peek();
                }
            }

            token = tokens.Dequeue();
            EnsureToken("FunctionStop", token, ExcelFormulaTokenType.Function, ExcelFormulaTokenSubtype.Stop);

            switch (name)
            {
                // EXCEL FUNCTIONS
                case "UPPER":
                    return arguments[0] == null ? null : arguments[0].ToString().ToUpper();
                case "LOWER":
                    return arguments[0] == null ? null : arguments[0].ToString().ToLower();
                case "PROPER":
                    return arguments[0] == null ? null : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arguments[0].ToString());
                case "IF":
                    return Convert.ToBoolean(arguments[0]) ? arguments[1] : arguments[2];

                // CUSTOM UDF FUNCTIONS
                case "SYSDATE":
                    return DateTime.Today.ToString("d/M/yyyy");
                case "SOURCE":
                    return sources[arguments[0].ToString()]?.SelectToken(arguments[1].ToString());
                case "RQD":
                    return sources["RQ"]?["RequestData"]?[arguments[0]];
                case "RQL":
                    return sources["RQ"]?["LogHeader"]?[arguments[0]];
                case "RQR":
                    return sources["#ROW#"]?[arguments[0]];
                case "CONTENT":
                    return Convert.ToBoolean(arguments[0]) ? "#EMPTY_CONTENT#" : null;
                default:
                    throw new InvalidOperationException($"Unknown function name: {name}");
            }
        }

        private object EvaluateOperand(Queue<ExcelFormulaToken> tokens, Dictionary<string, JToken> sources)
        {
            var token = tokens.Dequeue();
            return token.Value;
        }

        private void EnsureToken(string pos, ExcelFormulaToken token, ExcelFormulaTokenType type, ExcelFormulaTokenSubtype subType)
        {
            if (token.Type != type)
                throw new InvalidOperationException($"Invalid token type @ {pos}: Expected {type}, received {token.Type}");
            if (token.Subtype != subType)
                throw new InvalidOperationException($"Invalid token subtype @ {pos}: Expected {subType}, received {token.Subtype}");
        }
    }
}
