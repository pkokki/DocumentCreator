using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue ISEVEN(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotDecimal(0, null, out decimal number) || args[0] is ExcelValue.BooleanValue) 
                return ExcelValue.VALUE;
            var isEven = Math.Truncate(number) % 2 == 0;
            return isEven ? ExcelValue.TRUE : ExcelValue.FALSE;
        }
        public ExcelValue ISODD(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotDecimal(0, null, out decimal number) || args[0] is ExcelValue.BooleanValue)
                return ExcelValue.VALUE;
            var isOdd = Math.Truncate(number) % 2 != 0;
            return isOdd ? ExcelValue.TRUE : ExcelValue.FALSE;
        }

        public ExcelValue NA(List<ExcelValue> args, ExpressionScope scope)
        {
            return ExcelValue.NA;
        }

        public ExcelValue ISBLANK(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.RangeValue)
                return ExcelValue.BooleanValue.Create(scope.Contains(args[0].Text));
            return ExcelValue.FALSE;
        }

        public ExcelValue ISERR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.ErrorValue && args[0] != ExcelValue.NA)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISERROR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.ErrorValue)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISLOGICAL(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.BooleanValue)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISNA(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] == ExcelValue.NA)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISNONTEXT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && !(args[0] is ExcelValue.TextValue))
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISTEXT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.TextValue)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue ISNUMBER(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1 && args[0] is ExcelValue.DecimalValue)
                return ExcelValue.TRUE;
            return ExcelValue.FALSE;
        }

        public ExcelValue N(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1)
            {
                if (args[0] is ExcelValue.ErrorValue)
                    return args[0];
                if (args[0] == ExcelValue.TRUE)
                    return new ExcelValue.DecimalValue(1, scope.OutLanguage);
                if (args[0] is ExcelValue.DecimalValue)
                    return new ExcelValue.DecimalValue(args[0].AsDecimal().Value, scope.OutLanguage);
            }
            return new ExcelValue.DecimalValue(0, scope.OutLanguage);
        }

        public ExcelValue TYPE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count == 1)
            {
                if (args[0] is ExcelValue.DecimalValue)
                    return new ExcelValue.DecimalValue(1, scope.OutLanguage);
                if (args[0] is ExcelValue.TextValue)
                    return new ExcelValue.DecimalValue(2, scope.OutLanguage);
                if (args[0] is ExcelValue.BooleanValue)
                    return new ExcelValue.DecimalValue(4, scope.OutLanguage);
                if (args[0] is ExcelValue.ErrorValue)
                    return new ExcelValue.DecimalValue(16, scope.OutLanguage);
                if (args[0] is ExcelValue.ArrayValue)
                    return new ExcelValue.DecimalValue(64, scope.OutLanguage);
            }
            return ExcelValue.VALUE;
        }
    }
}
