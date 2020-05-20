using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {

        private ExcelValue Math1(List<ExcelValue> args, ExpressionScope scope, Func<double, double> oper, Func<double, bool> guard = null)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (guard != null && guard(number)) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(oper(number), scope.OutLanguage);
        }
        private ExcelValue Math2(List<ExcelValue> args, ExpressionScope scope, Func<double, double, double> oper, Func<double, double, bool> guard = null)
        {
            if (args.NotDecimal(0, null, out double n1)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out double n2)) return ExcelValue.VALUE;
            if (guard != null && guard(n1, n2)) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(oper(n1, n2), scope.OutLanguage);
        }

        public ExcelValue ACOS(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Acos(v), v => v < -1 || v > 1);
        public ExcelValue ACOSH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Acosh(v), v => v <= -1 || v == 0);
        public ExcelValue ASIN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Asin(v), v => v < -1 || v > 1);
        public ExcelValue ASINH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Asinh(v));
        public ExcelValue ACOT(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.PI / 2 - Math.Atan(v));
        public ExcelValue ACOTH(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (number == 1) return ExcelValue.VALUE;
            var d = (number + 1) / (number - 1);
            if (d <= 0) return ExcelValue.VALUE;
            return new ExcelValue.DecimalValue(Math.Log(d) / 2, scope.OutLanguage);
        }
        public ExcelValue ATAN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Atan(v));
        public ExcelValue ATAN2(List<ExcelValue> args, ExpressionScope scope) => Math2(args, scope,
            (v1, v2) => Math.Atan2(v2, v1),
            (v1, v2) => v1 == 0 && v2 == 0
            );
        public ExcelValue ATANH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Atanh(v), v => v <= -1 || v >= 1);
        public ExcelValue COS(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Cos(v));
        public ExcelValue COSH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Cosh(v));
        public ExcelValue COT(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 1.0 / Math.Tan(v), v => v == 0);
        public ExcelValue COTH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => (Math.Exp(v) + Math.Exp(-v)) / (Math.Exp(v) - Math.Exp(-v)));
        public ExcelValue CSC(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 1.0 / Math.Sin(v));
        public ExcelValue CSCH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 2.0 / (Math.Exp(v) - Math.Exp(-v)));
        public ExcelValue PI(List<ExcelValue> args, ExpressionScope scope) => new ExcelValue.DecimalValue(Math.PI, scope.OutLanguage);
        public ExcelValue SEC(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 1.0 / Math.Cos(v));
        public ExcelValue SECH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 2.0 / (Math.Exp(v) + Math.Exp(-v)));
        public ExcelValue SIN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Sin(v));
        public ExcelValue SINH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Sinh(v));
        public ExcelValue TAN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Tan(v));
        public ExcelValue TANH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Tanh(v));

        
        public ExcelValue ABS(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Abs(v));
        public ExcelValue CEILING(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out double significance)) return ExcelValue.VALUE;
            if (significance == 0)
                return ExcelValue.ZERO;
            var ceiling = Math.Ceiling(number / significance) * significance;
            return new ExcelValue.DecimalValue(ceiling, scope.OutLanguage);
        }
        public ExcelValue CEILING_MATH(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            var significance = 0.0;
            if (number >= 0 && args.NotDecimal(1, 1, out significance)) return ExcelValue.VALUE;
            if (number < 0 && args.NotDecimal(1, -1, out significance)) return ExcelValue.VALUE;
            if (args.NotInteger(2, 0, out int mode)) return ExcelValue.VALUE;
            if (significance == 0)
                return ExcelValue.ZERO;
            significance = Math.Abs(significance);

            double ceiling;
            if (mode == 0 || number >= 0)
                ceiling = Math.Ceiling(number / significance) * significance;
            else
                ceiling = Math.Floor(number / significance) * significance;
            return new ExcelValue.DecimalValue(ceiling, scope.OutLanguage);
        }
        public ExcelValue CEILING_PRECISE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, 1, out double significance)) return ExcelValue.VALUE;
            if (significance == 0)
                return ExcelValue.ZERO;

            var sign = Math.Sign(number);
            number = Math.Abs(number);
            significance = Math.Abs(significance);
            double ceiling;
            if (sign < 0)
                ceiling = Math.Floor(number / significance) * significance;
            else
                ceiling = Math.Ceiling(number / significance) * significance;
            return new ExcelValue.DecimalValue(sign * ceiling, scope.OutLanguage);
        }
        public ExcelValue FLOOR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out double significance)) return ExcelValue.VALUE;
            if (significance == 0 || (number > 0 && significance < 0))
                return ExcelValue.VALUE;
            var ceiling = Math.Floor(number / significance) * significance;
            return new ExcelValue.DecimalValue(ceiling, scope.OutLanguage);
        }
        public ExcelValue FLOOR_MATH(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            var significance = 0.0;
            if (number >= 0 && args.NotDecimal(1, 1, out significance)) return ExcelValue.VALUE;
            if (number < 0 && args.NotDecimal(1, -1, out significance)) return ExcelValue.VALUE;
            if (args.NotInteger(2, 0, out int mode)) return ExcelValue.VALUE;
            if (significance == 0)
                return ExcelValue.ZERO;
            significance = Math.Abs(significance);

            double ceiling;
            if (mode == 0 || number >= 0)
                ceiling = Math.Floor(number / significance) * significance;
            else
                ceiling = Math.Ceiling(number / significance) * significance;
            return new ExcelValue.DecimalValue(ceiling, scope.OutLanguage);
        }
        public ExcelValue FLOOR_PRECISE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, 1, out double significance)) return ExcelValue.VALUE;
            if (significance == 0)
                return ExcelValue.ZERO;

            var sign = Math.Sign(number);
            number = Math.Abs(number);
            significance = Math.Abs(significance);
            double ceiling;
            if (sign < 0)
                ceiling = Math.Ceiling(number / significance) * significance;
            else
                ceiling = Math.Floor(number / significance) * significance;
            return new ExcelValue.DecimalValue(sign * ceiling, scope.OutLanguage);
        }

        public ExcelValue ROUND(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotInteger(1, null, out int decimals)) return ExcelValue.VALUE;

            double round;
            if (decimals < 0)
            {
                var factor = Math.Pow(10, -decimals);
                round = Math.Round(number / factor) * factor;
            }
            else
            {
                round = Math.Round(number, decimals);
            }
            return new ExcelValue.DecimalValue(round, scope.OutLanguage);
        }
        public ExcelValue ROUNDDOWN(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotInteger(1, null, out int decimals)) return ExcelValue.VALUE;

            double round;
            if (decimals < 0)
            {
                var factor = Math.Pow(10, -decimals);
                round = Math.Truncate(number / factor) * factor;
            }
            else
            {
                var factor = Math.Pow(10, decimals);
                round = Math.Truncate(number * factor) / factor;
            }
            return new ExcelValue.DecimalValue(round, scope.OutLanguage);
        }
        public ExcelValue ROUNDUP(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotInteger(1, null, out int decimals)) return ExcelValue.VALUE;

            double round;
            if (decimals < 0)
            {
                var factor = Math.Pow(10, -decimals);
                if (number > 0)
                    round = Math.Ceiling(number / factor) * factor;
                else
                    round = Math.Floor(number / factor) * factor;
            }
            else
            {
                var factor = Math.Pow(10, decimals);
                if (number > 0)
                    round = Math.Ceiling(number * factor) / factor;
                else
                    round = Math.Floor(number * factor) / factor;
            }
            return new ExcelValue.DecimalValue(round, scope.OutLanguage);
        }

        public ExcelValue TRUNC(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, 0, out double digits)) return ExcelValue.VALUE;

            var factor = Math.Pow(10, digits);
            var trunc = Math.Truncate(number * factor) / factor;
            return new ExcelValue.DecimalValue(trunc, scope.OutLanguage);
        }

        public ExcelValue EXP(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Exp(v));
        public ExcelValue INT(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Floor(v));
        public ExcelValue LN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Log(v), v => v <= 0);
        public ExcelValue LOG(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, 10, out double newBase)) return ExcelValue.VALUE;
            if (number <= 0) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(Math.Log(number, newBase), scope.OutLanguage);
        }
        public ExcelValue LOG10(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Log10(v), v => v <= 0);
        public ExcelValue MOD(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out double number)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out double divisor)) return ExcelValue.VALUE;
            if (divisor == 0) return ExcelValue.VALUE;

            var mod = number - divisor * Math.Floor(number / divisor);
            return new ExcelValue.DecimalValue(mod, scope.OutLanguage);
        }
        public ExcelValue POWER(List<ExcelValue> args, ExpressionScope scope) => Math2(args, scope, (v1, v2) => Math.Pow(v1, v2));
        public ExcelValue RAND(List<ExcelValue> args, ExpressionScope scope) => new ExcelValue.DecimalValue(random.Value.NextDouble(), scope.OutLanguage);
        public ExcelValue RANDBETWEEN(List<ExcelValue> args, ExpressionScope scope) => Math2(args, scope, 
            (v1, v2) => random.Value.Next((int)Math.Ceiling(v1), (int)Math.Round(v2)),
            (v1, v2) => v1 > v2);
        public ExcelValue SIGN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Sign(v));


        public ExcelValue SUM(List<ExcelValue> args, ExpressionScope scope)
        {
            var numbers = args.FlattenNumbers(false);
            if (numbers == null) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(numbers.Sum(), scope.OutLanguage);
        }
        public ExcelValue SUMIF(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out IEnumerable<ExcelValue> range)) return ExcelValue.VALUE;
            var criteria = args.Count > 1 ? args[1] : null;
            if (criteria == null) return ExcelValue.NA;
            if (args.NotArray(2, range, out IEnumerable<ExcelValue> sum_range)) return ExcelValue.VALUE;

            var selected = new List<ExcelValue>();
            var filter = ExcelCriteria.Resolve(criteria, scope.OutLanguage);
            foreach (var pair in range.Zip(sum_range, (a, b) => new { Predicate = a, Number = b }))
            {
                if (filter(pair.Predicate))
                    selected.Add(pair.Number);
            }

            var numbers = selected.FlattenNumbers(false);
            if (numbers == null) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(numbers.Sum(), scope.OutLanguage);
        }

        

        public ExcelValue PRODUCT(List<ExcelValue> args, ExpressionScope scope)
        {
            var numbers = args.FlattenNumbers(false);
            if (numbers == null) return ExcelValue.VALUE;

            var product = numbers.Aggregate(1.0, (acc, val) => acc * val); ;
            return new ExcelValue.DecimalValue(product, scope.OutLanguage);
        }
    }
}
