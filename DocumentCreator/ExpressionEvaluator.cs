﻿using DocumentCreator.ExcelFormulaParser;
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
        private readonly Language inputLang, outputLang;

        public ExpressionEvaluator() : this(Language.Invariant, Language.ElGr)
        {
        }

        public ExpressionEvaluator(Language inputLang, Language outputLang)
        {
            this.inputLang = inputLang;
            this.outputLang = outputLang;
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
        
        public IEnumerable<EvaluationResult> Evaluate(ICollection<TemplateFieldExpression> templateFieldExpressions, Dictionary<string, JToken> sources)
        {
            return Evaluate(templateFieldExpressions, new ExpressionScope(inputLang, outputLang, sources));
        }

        public EvaluationResult Evaluate(string exprName, string expression, Dictionary<string, JToken> sources)
        {
            return Evaluate(exprName, expression, new ExpressionScope(inputLang, outputLang, sources));
        }

        private IEnumerable<EvaluationResult> Evaluate(ICollection<TemplateFieldExpression> templateFieldExpressions, ExpressionScope scope)
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
                {
                    if (!expr.StartsWith("="))
                        expr = "=" + expr;
                    var exprName = templateFieldExpression.Cell ?? templateFieldExpression.Name;
                    result = Evaluate(exprName, expr, scope);
                    templateFieldExpression.Result = result;
                }
                results.Add(result);
            }
            return results;
        }

        private EvaluationResult Evaluate(string exprName, string expression, ExpressionScope scope)
        {
            var excelFormula = new ExcelFormula(expression, scope.InLanguage);
            var tokens = excelFormula.OfType<ExcelFormulaToken>();
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
                scope.Set(exprName, operand.Value);
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
