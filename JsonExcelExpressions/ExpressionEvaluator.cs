using JsonExcelExpressions.Eval;
using JsonExcelExpressions.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JsonExcelExpressions
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly Language outputLang;

        public ExpressionEvaluator() : this(CultureInfo.CurrentCulture)
        {
        }

        public ExpressionEvaluator(CultureInfo culture) : this(Language.Create(culture))
        {
        }

        private ExpressionEvaluator(Language outputLang)
        {
            this.outputLang = outputLang ?? Language.Invariant;
        }

        public EvaluationResult Evaluate(string expression, CultureInfo culture = null)
        {
            return Evaluate(expression, new JObject(), culture);
        }

        public EvaluationResult Evaluate(string expression, JObject source, CultureInfo culture = null)
        {
            var results = Evaluate(new List<string>() { expression }, source, culture);
            return results.Last();
        }

        public IEnumerable<EvaluationResult> Evaluate(IEnumerable<string> expressions, JObject source, CultureInfo culture = null)
        {
            var sourcePayload = source ?? new JObject();
            var sourceName = "inp";
            var sources = new List<EvaluationSource>() { new EvaluationSource() { Name = sourceName, Cell = "N3", Payload = sourcePayload } };
            var results = new List<EvaluationResult>();
            var scope = new ExpressionScope(culture == null ? outputLang : Language.Create(culture), sources);
            var helper = new JsonExpressionHelper();
            var index = 1;
            foreach (var expression in expressions)
            {
                var exprName = $"__A{index}";
                var cell = exprName;
                var tokens = helper.Parse(sourcePayload, expression);
                var result = Evaluate(exprName, cell, tokens, scope);
                var translatedResult = helper.TranslateResult(result);
                results.Add(translatedResult);
                ++index;
            }
            return results;
        }

        protected EvaluationResult Evaluate(string exprName, string cell, string expression, ExpressionScope scope)
        {
            if (!expression.StartsWith("="))
                expression = "=" + expression;
            var excelFormula = new ExcelFormula(expression);
            var tokens = excelFormula.OfType<ExcelFormulaToken>();
            return Evaluate(exprName, cell, tokens, scope);
        }

        internal EvaluationResult Evaluate(string exprName, string cell, IEnumerable<ExcelFormulaToken> tokens, ExpressionScope scope)
        {
            var result = new EvaluationResult()
            {
                Name = exprName
            };
            try
            {
                var queue = new Queue<ExcelFormulaToken>(tokens);
                var excelExpression = new ExcelExpression();
                TraverseExpression(excelExpression, queue, scope);
                var operand = excelExpression.Evaluate(scope);
                scope.Set(cell ?? exprName, operand.Value);
                result.Value = operand.Value.InnerValue;
                result.Text = outputLang.ToString(operand.Value);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }
            finally { }
            return result;
        }

        

        private void TraverseExpression(ExcelExpression expression, Queue<ExcelFormulaToken> tokens, ExpressionScope scope)
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
                    TraverseExpression(childExpression, tokens, scope);
                    switch (token.Type)
                    {
                        case ExcelFormulaTokenType.Function:
                            var name = token.Value.ToUpper();
                            var args = EvaluateFunctionArguments(childExpression, scope)
                                .Select(o => o.Value)
                                .ToList();
                            var value = Functions.INSTANCE.Evaluate(name, args, scope);
                            expression.Add(new ExcelExpressionPart(value));
                            break;
                        case ExcelFormulaTokenType.Subexpression:
                            var subExpression = childExpression.Evaluate(scope);
                            expression.Add(subExpression);
                            break;
                        default:
                            throw new NotImplementedException($"Start TokenType={token.Type}");
                    }
                }
                else
                {
                    expression.Add(new ExcelExpressionPart(token, scope));
                }
            }
        }

        private IEnumerable<ExcelExpressionPart> EvaluateFunctionArguments(ExcelExpression expression, ExpressionScope scope)
        {
            var args = new List<ExcelExpressionPart>();
            ExcelExpression activeArg = null;
            foreach (var item in expression)
            {
                if (item.TokenType == ExcelFormulaTokenType.Argument)
                {
                    if (activeArg == null)
                    {
                        // possible optional argument -- consecutive commas
                        args.Add(new ExcelExpressionPart(ExcelValue.NULL));
                    }
                    else
                    {
                        args.Add(activeArg.Evaluate(scope));
                    }
                    activeArg = null;
                }
                else
                {
                    activeArg ??= new ExcelExpression();
                    activeArg.Add(item);
                }
            }
            if (activeArg != null) args.Add(activeArg.Evaluate(scope));
            return args;
        }

        protected ExpressionScope CreateExpressionScope(IEnumerable<EvaluationSource> sources)
        {
            return new ExpressionScope(outputLang, sources);
        }


    }
}
