using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DocumentCreator
{
    public class ExpressionEvaluator
    {
        private readonly CultureInfo culture, outCulture;

        public ExpressionEvaluator() : this(CultureInfo.InvariantCulture, CultureInfo.CurrentCulture)
        {
        }

        public ExpressionEvaluator(CultureInfo culture, CultureInfo outCulture)
        {
            this.culture = culture;
            this.outCulture = outCulture;
        }

        public ExpressionResult Evaluate(long targetId, string expression, Dictionary<string, JToken> sources)
        {
            sources ??= new Dictionary<string, JToken>();
            var excelFormula = new ExcelFormula(expression, culture);
            var tokens = excelFormula.OfType<ExcelFormulaToken>();
            var repetitions = 1;
            if (tokens.Any(t => t.Type == ExcelFormulaTokenType.Function
                && string.Equals(t.Value, "RQR", StringComparison.InvariantCultureIgnoreCase)))
            {
                repetitions = sources["#COLL#"].Count();
            }
            var result = new ExpressionResult()
            {
                TargetId = targetId,
                Expression = expression,
            };
            try
            {
                for (var i = 0; i < repetitions; i++)
                {
                    sources["#ROW#"] = sources.ContainsKey("#COLL#") ? sources["#COLL#"]?.Skip(i).FirstOrDefault() : null;
                    var queue = new Queue<ExcelFormulaToken>(tokens);
                    var excelExpression = new ExcelExpression();
                    TraverseExpression(excelExpression, queue, sources);
                    var operand = excelExpression.Evaluate();
                    if (i == 0)
                    {
                        var value = operand.Value;
                        if (value.InnerValue is JArray collection)
                        {
                            sources["#COLL#"] = collection;
                            result.ChildRows = collection.Count;
                        }
                        result.Value = value?.ToString(outCulture);
                    }
                    result.Rows.Add(operand.Value?.ToString(outCulture));
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            return result;
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
