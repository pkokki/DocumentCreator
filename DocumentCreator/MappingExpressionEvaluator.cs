using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using JsonExcelExpressions;
using JsonExcelExpressions.Eval;
using JsonExcelExpressions.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DocumentCreator
{
    public class MappingExpressionEvaluator : ExpressionEvaluator, IMappingExpressionEvaluator
    {
        public MappingExpressionEvaluator() : base()
        {
        }

        public MappingExpressionEvaluator(CultureInfo culture) : base(culture)
        {
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

        public IEnumerable<EvaluationResult> Evaluate(IEnumerable<MappingExpression> templateFieldExpressions, IEnumerable<EvaluationSource> sources)
        {
            return Evaluate(templateFieldExpressions, CreateExpressionScope(sources));
        }


        public EvaluationResult Evaluate(string exprName, string cell, string expression, IEnumerable<EvaluationSource> sources)
        {
            return Evaluate(exprName, cell, expression, CreateExpressionScope(sources));
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
