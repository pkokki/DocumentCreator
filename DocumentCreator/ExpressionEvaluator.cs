using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentCreator.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator
{
    public class ExpressionEvaluator
    {
        private readonly ExpressionContext context;

        public ExpressionEvaluator() : this(Language.Invariant, Language.ElGr)
        {
        }

        public ExpressionEvaluator(Language inputLang, Language outputLang)
        {
            context = new ExpressionContext(inputLang, outputLang);
        }

        public EvaluationResponse Evaluate(EvaluationRequest request, IEnumerable<TemplateField> templateFields)
        {
            var expressions = new List<TemplateFieldExpression>(request.Expressions);
            var sourceDict = new Dictionary<string, JToken>();
            if (request.Sources != null)
                foreach (var source in request.Sources)
                    sourceDict[source.Name] = source.Payload;

            PreEvaluate(expressions, templateFields);
            Evaluate(expressions, sourceDict);
            PostEvaluate(expressions);
        
            var response = new EvaluationResponse()
            {
                Total = expressions.Count(o => !string.IsNullOrEmpty(o.Expression)),
                Errors = expressions.Count(o => !string.IsNullOrEmpty(o.Result.Error)),
                Results = expressions.Select(o => o.Result)
            };
            return response;
        }

        
        // TODO: Refactor to return IEnumerable<ExpressionResult>
        public void Evaluate(ICollection<TemplateFieldExpression> templateFieldExpressions, Dictionary<string, JToken> sources)
        {
            sources ??= new Dictionary<string, JToken>();
            sources["#OWN#"] = new JObject();
            foreach (var templateFieldExpression in templateFieldExpressions)
            {
                var expr = templateFieldExpression.Expression;
                if (string.IsNullOrWhiteSpace(expr) || expr == "=")
                {
                    templateFieldExpression.Result = new ExpressionResult()
                    {
                        Name = templateFieldExpression.Name,
                    };
                }
                else
                {
                    if (!expr.StartsWith("="))
                        expr = "=" + expr;
                    var exprName = templateFieldExpression.Cell ?? templateFieldExpression.Name;
                    var result = Evaluate(exprName, expr, sources);
                    templateFieldExpression.Result = result;
                }
            }
        }

        public ExpressionResult Evaluate(string exprName, string expression, Dictionary<string, JToken> sources)
        {
            sources ??= new Dictionary<string, JToken>();
            var excelFormula = new ExcelFormula(expression, context);
            var tokens = excelFormula.OfType<ExcelFormulaToken>();
            var repetitions = 1;
            if (tokens.Any(t => t.Type == ExcelFormulaTokenType.Function
                && string.Equals(t.Value, "RQR", StringComparison.InvariantCultureIgnoreCase)))
            {
                repetitions = sources["#COLL#"].Count();
            }
            var result = new ExpressionResult()
            {
                Name = exprName,
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
                    var operand = excelExpression.Evaluate(sources, context.OutputLang);
                    if (i == 0)
                    {
                        var value = operand.Value;
                        if (value.InnerValue is JArray collection)
                        {
                            sources["#COLL#"] = collection;
                            result.ChildRows = collection.Count;
                        }
                        if (sources.ContainsKey("#OWN#"))
                            sources["#OWN#"][exprName] = value.InnerValue != null ? JToken.FromObject(value.InnerValue) : null;
                        result.Value = value.InnerValue; //################ context.OutputLang.ToString(value);
                        result.Text = context.OutputLang.ToString(value);
                    }
                    result.Rows.Add(context.OutputLang.ToString(operand.Value));
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
                            var args = EvaluateFunctionArguments(childExpression, sources)
                                .Select(o => o.Value)
                                .ToList();
                            var value = Functions.INSTANCE.Evaluate(name, args, context.OutputLang, sources);
                            expression.Add(new ExcelExpressionPart(value));
                            break;
                        case ExcelFormulaTokenType.Subexpression:
                            var subExpression = childExpression.Evaluate(sources, context.OutputLang);
                            expression.Add(subExpression);
                            break;
                        default:
                            throw new NotImplementedException($"Start TokenType={token.Type}");
                    }
                }
                else
                {
                    expression.Add(new ExcelExpressionPart(token, context));
                }
            }
        }

        private IEnumerable<ExcelExpressionPart> EvaluateFunctionArguments(ExcelExpression expression, IDictionary<string, JToken> sources)
        {
            var args = new List<ExcelExpressionPart>();
            ExcelExpression activeArg = null;
            foreach (var item in expression)
            {
                if (item.TokenType == ExcelFormulaTokenType.Argument)
                {
                    args.Add(activeArg.Evaluate(sources, context.OutputLang));
                    activeArg = null;
                }
                else
                {
                    activeArg ??= new ExcelExpression();
                    activeArg.Add(item);
                }
            }
            if (activeArg != null) args.Add(activeArg.Evaluate(sources, context.OutputLang));
            return args;
        }

        private void PreEvaluate(List<TemplateFieldExpression> expressions, IEnumerable<TemplateField> templateFields)
        {
            if (templateFields != null)
            {
                foreach (var expression in expressions)
                {
                    expression.Content = templateFields.FirstOrDefault(o => o.Name == expression.Name)?.Content;
                }
            }
        }

        private void PostEvaluate(IEnumerable<TemplateFieldExpression> expressions)
        {
            foreach (var expression in expressions)
            {
                if (expression.IsCollection)
                {
                    expressions
                        .Where(o => o.Parent == expression.Name)
                        .ToList()
                        .ForEach(o =>
                        {
                            o.Result.Text = new JArray(o.Result.Rows).ToString(Newtonsoft.Json.Formatting.None).Replace("\"", "'");
                        });
                }
                else if (!string.IsNullOrEmpty(expression.Parent))
                {
                    expression.Result.Text = new JArray(expression.Result.Rows).ToString(Newtonsoft.Json.Formatting.None).Replace("\"", "'");
                }
                else
                {
                    if (expression.Result.Text == "#HIDE_CONTENT#")
                    {
                        expression.Result.Text = string.Empty;
                    }
                    else if (expression.Result.Text == "#SHOW_CONTENT#")
                    {
                        expression.Result.Text = expression.Content;
                    }
                }
            }
        }
    }
}
