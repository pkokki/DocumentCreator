using System.Collections.Generic;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue AND(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Any(a => !a.AsBoolean().HasValue)) return ExcelValue.NA;
            return new ExcelValue.BooleanValue(args.All(o => o.AsBoolean().Value));
        }

        public ExcelValue IF(List<ExcelValue> args, ExpressionScope scope)
        {
            if (!args[0].AsBoolean().HasValue) return ExcelValue.NA;
            return args[0].AsBoolean().Value ? args[1] : args[2];
        }

        public ExcelValue IFERROR(List<ExcelValue> args, ExpressionScope scope)
        {
            return args[0] is ExcelValue.ErrorValue ? args[1] : args[0];
        }

        public ExcelValue IFNA(List<ExcelValue> args, ExpressionScope scope)
        {
            return ExcelValue.NA.Equals(args[0]) ? args[1] : args[0];
        }

        public ExcelValue NOT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args[0] is ExcelValue.TextValue) return ExcelValue.VALUE;
            if (!args[0].AsBoolean().HasValue) return ExcelValue.NA;
            return new ExcelValue.BooleanValue(!args[0].AsBoolean().Value);
        }

        public ExcelValue OR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Any(a => !a.AsBoolean().HasValue)) return ExcelValue.NA;
            return new ExcelValue.BooleanValue(args.Any(o => o.AsBoolean().Value));
        }

        public ExcelValue XOR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Any(a => !a.AsBoolean().HasValue)) return ExcelValue.NA;
            return new ExcelValue.BooleanValue(args.Select(o => o.AsBoolean().Value).Aggregate((a, b) => a ^ b));
        }
    }
}
