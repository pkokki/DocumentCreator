using System.Collections.Generic;
using System.Linq;
using static DocumentCreator.ExcelFormulaParser.ExcelValue;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue PI(List<ExcelValue> args, ExpressionScope scope)
        {
            return new DecimalValue(3.14159265358979M, scope.OutLanguage);
        }

        public ExcelValue SUM(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var result = 0M;
            foreach (var arg in args)
            {
                if (arg is ArrayValue)
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
            return new DecimalValue(result, scope.OutLanguage);
        }
    }
}
