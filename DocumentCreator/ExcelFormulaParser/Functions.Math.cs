using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue SUM(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var result = 0M;
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
