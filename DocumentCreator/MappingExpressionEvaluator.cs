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

        public IEnumerable<EvaluationResult> Evaluate(IEnumerable<MappingExpression> expressions, IEnumerable<EvaluationSource> sources)
        {
            var results = new List<EvaluationResult>();
         
            var scope = CreateExpressionScope(sources);
            foreach (var expression in expressions)
            {
                var result = new EvaluationResult()
                {
                    Name = expression.Name,
                    Cell = expression.Cell
                };
                scope.ParentName = expressions.FirstOrDefault(o => o.Name == expression.Parent)?.Cell;
                var exprFormula = expression.Expression;
                if (!string.IsNullOrWhiteSpace(exprFormula) && exprFormula != "=")
                {
                    var format = CreateExpressionFormat(expression.NumFormatId, expression.NumFormatCode);
                    result = Evaluate(expression.Name, expression.Cell, exprFormula, scope, format);
                }
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
                var expression = expressions.First(o => o.Name == result.Name);
                if (result.Text == "#HIDE_CONTENT#")
                {
                    result.Text = string.Empty;
                }
                else if (result.Text == "#SHOW_CONTENT#")
                {
                    result.Text = expression.Content;
                }
            }
        }

        
    }
}
