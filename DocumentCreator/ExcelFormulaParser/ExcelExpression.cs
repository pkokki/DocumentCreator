using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExcelExpression : List<ExcelExpressionPart>
    {
        public ExcelExpressionPart Evaluate()
        {
            // https://support.office.com/en-us/article/calculation-operators-and-precedence-in-excel-48be406d-4975-4d31-b2b8-7af9e0e2878a
            PerformNegation();
            ConvertPercentages();
            PerformExponentiation();
            PerformMultiplicationAndDivision();
            PerformAdditionAndSubtraction();
            EvaluateTextOperators();
            PerformComparisons();

            var value = this.Single().Value;
            //if (value is string && decimal.TryParse((string)value, NumberStyles.Any, culture, out decimal d))
            //    value = d;
            return new ExcelExpressionPart(value);
        }

        private void PerformNegation()
        {
            ReplaceWithNext(part => part.IsPrefixOperator("-"), operand => -operand);
        }
        private void ConvertPercentages()
        {
            ReplaceWithPrevious(part => part.IsPostfixOperator("%"), operand => operand / 100M);
        }
        private void PerformExponentiation()
        {
            ReplaceTriplet(part => part.IsBinaryOperator("^"), (oper, a, b) => a ^ b);
        }
        private void PerformMultiplicationAndDivision()
        {
            ReplaceTriplet(part => part.IsBinaryOperator("*", "/"), (oper, a, b) => oper == "*" ? a * b : a / b);
        }
        private void PerformAdditionAndSubtraction()
        {
            ReplaceTriplet(part => part.IsBinaryOperator("+", "-"), (oper, a, b) => oper == "+" ? a + b : a - b);
        }
        private void EvaluateTextOperators()
        {
            ReplaceTriplet(part => part.IsBinaryOperator("&"), (oper, a, b) => a & b);
        }
        private void PerformComparisons()
        {
            ReplaceTriplet(part => part.IsComparisonOperator, (oper, a, b) => ExcelValue.CreateBoolean(oper, a, b));
        }

        private void ReplaceTriplet(Predicate<ExcelExpressionPart> selector, Func<string, ExcelValue, ExcelValue, ExcelValue> calculate)
        {
            var index = FindIndex(p => selector(p));
            while (index > -1)
            {
                var a = this[index - 1].Value;
                var oper = GetAndRemoveAt(index).Operator;
                var b = GetAndRemoveAt(index).Value;

                var result = calculate(oper, a, b);
                this[index - 1] = new ExcelExpressionPart(result);

                index = FindIndex(p => selector(p));
            }
        }
        private void ReplaceWithNext(Predicate<ExcelExpressionPart> selector, Func<ExcelValue, ExcelValue> calculate)
        {
            var index = FindIndex(o => selector(o));
            while (index > -1)
            {
                var operand = GetAndRemoveAt(index + 1);
                this[index] = new ExcelExpressionPart(calculate(operand.Value));
                index = FindIndex(o => selector(o));
            }
        }
        private void ReplaceWithPrevious(Predicate<ExcelExpressionPart> selector, Func<ExcelValue, ExcelValue> calculate)
        {
            var index = FindIndex(o => selector(o));
            while (index > -1)
            {
                RemoveAt(index);
                var operand = this[index - 1];
                this[index - 1] = new ExcelExpressionPart(calculate(operand.Value));
                index = FindIndex(o => selector(o));
            }
        }

        private ExcelExpressionPart GetAndRemoveAt(int index)
        {
            var item = this[index];
            RemoveAt(index);
            return item;
        }
    }
}
