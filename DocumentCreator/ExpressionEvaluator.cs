using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.ExcelFormulaParser;
using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private readonly Language inputLang, outputLang;

        public ExpressionEvaluator() : this(Language.Invariant, Language.ElGr)
        {
        }

        public ExpressionEvaluator(Language inputLang, Language outputLang)
        {
            this.inputLang = inputLang;
            this.outputLang = outputLang;
        }

        public IEnumerable<EvaluationResult> Evaluate(ExpressionEvaluationInput input) 
        {
            var expressions = input.Expressions ?? new string[0];
            var sourcePayload = input.Payload ?? new JObject();
            var sourceName = "inp";
            var sources = new List<MappingSource>() { new MappingSource() { Name = sourceName, Cell = "N3", Payload = sourcePayload } };
            var results = new List<EvaluationResult>();
            var scope = new ExpressionScope(Language.Invariant, Language.ElGr, sources);
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

        public EvaluationResult Evaluate(string expression, JObject payload)
        {
            var results = Evaluate(new ExpressionEvaluationInput() 
            {
                Expressions = new List<string>() { expression }, 
                Payload = payload
            });
            return results.Last();
        }

        public EvaluationOutput Evaluate(EvaluationInput input)
        {
            var expressions = new List<MappingExpression>(input.Expressions);
            PreEvaluate(expressions, input.Fields);
            var results = Evaluate(expressions, input.Sources);
            PostEvaluate(expressions, results);

            var response = new EvaluationOutput()
            {
                Total = expressions.Count(o => !string.IsNullOrEmpty(o.Expression)),
                Errors = results.Count(o => !string.IsNullOrEmpty(o.Error)),
                Results = results
            };
            return response;
        }

        public IEnumerable<EvaluationResult> Evaluate(IEnumerable<MappingExpression> templateFieldExpressions, IEnumerable<MappingSource> sources)
        {
            return Evaluate(templateFieldExpressions, new ExpressionScope(inputLang, outputLang, sources));
        }


        public EvaluationResult Evaluate(string exprName, string cell, string expression, IEnumerable<MappingSource> sources)
        {
            return Evaluate(exprName, cell, expression, new ExpressionScope(inputLang, outputLang, sources));
        }

        private IEnumerable<EvaluationResult> Evaluate(IEnumerable<MappingExpression> templateFieldExpressions, ExpressionScope scope)
        {
            var results = new List<EvaluationResult>();
            foreach (var templateFieldExpression in templateFieldExpressions)
            {
                var result = new EvaluationResult()
                {
                    Name = templateFieldExpression.Name,
                };
                scope.ParentName = templateFieldExpressions.FirstOrDefault(o => o.Name == templateFieldExpression.Parent)?.Cell;
                var expr = templateFieldExpression.Expression;
                if (!string.IsNullOrWhiteSpace(expr) && expr != "=")
                    result = Evaluate(templateFieldExpression.Name, templateFieldExpression.Cell, expr, scope);
                results.Add(result);
            }
            return results;
        }

        private EvaluationResult Evaluate(string exprName, string cell, string expression, ExpressionScope scope)
        {
            if (!expression.StartsWith("="))
                expression = "=" + expression;
            var excelFormula = new ExcelFormulaParser.ExcelFormula(expression, scope.InLanguage);
            var tokens = excelFormula.OfType<ExcelFormulaToken>();
            return Evaluate(exprName, cell, tokens, scope);
        }
        private EvaluationResult Evaluate(string exprName, string cell, IEnumerable<ExcelFormulaToken> tokens, ExpressionScope scope)
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

        private void PreEvaluate(List<MappingExpression> expressions, IEnumerable<TemplateField> templateFields)
        {
            if (templateFields != null)
            {
                foreach (var expression in expressions)
                {
                    expression.Content = templateFields.FirstOrDefault(o => o.Name == expression.Name)?.Content;
                }
            }
        }

        private void PostEvaluate(IEnumerable<MappingExpression> expressions, IEnumerable<EvaluationResult> results)
        {
            foreach (var result in results)
            {
                if (result.Text == "#HIDE_CONTENT#")
                {
                    result.Text = string.Empty;
                }
                else if (result.Text == "#SHOW_CONTENT#")
                {
                    result.Text = expressions.First(o => o.Name == result.Name).Content;
                }
            }
        }
    }
}
