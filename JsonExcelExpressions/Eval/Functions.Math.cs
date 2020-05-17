using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue PI(List<ExcelValue> args, ExpressionScope scope)
        {
            return new ExcelValue.DecimalValue(/*3.14159265358979*/Math.PI, scope.OutLanguage);
        }

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

        public ExcelValue ABS(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Abs(v));
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
        public ExcelValue COTH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => (Math.Exp(v) + Math.Exp(-v)) /(Math.Exp(v) - Math.Exp(-v)));
        public ExcelValue CSC(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 1.0 / Math.Sin(v));
        public ExcelValue CSCH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 2.0 / (Math.Exp(v) - Math.Exp(-v)));
        public ExcelValue SEC(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 1.0 / Math.Cos(v));
        public ExcelValue SECH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => 2.0 / (Math.Exp(v) + Math.Exp(-v)));
        public ExcelValue SIN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Sin(v));
        public ExcelValue SINH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Sinh(v));
        public ExcelValue TAN(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Tan(v));
        public ExcelValue TANH(List<ExcelValue> args, ExpressionScope scope) => Math1(args, scope, v => Math.Tanh(v));

        public ExcelValue SUM(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var result = 0.0;
            foreach (var arg in args)
            {
                if (arg is ExcelValue.ArrayValue)
                {
                    ((IEnumerable<ExcelValue>)arg.InnerValue).ToList().ForEach(o => result += o.AsDecimal().Value);
                }
                else
                {
                    if (!arg.AsDecimal().HasValue)
                        return ExcelValue.VALUE;
                    result += arg.AsDecimal().Value;
                }
            }
            return new ExcelValue.DecimalValue(result, scope.OutLanguage);
        }
    }
}
