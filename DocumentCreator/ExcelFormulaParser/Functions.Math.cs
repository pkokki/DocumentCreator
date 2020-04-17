using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue SUM(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var result = 0M;
            foreach (var arg in args)
            {
                if (arg is ExcelValue.JsonTextValue && arg.InnerValue is JArray)
                {
                    ((JArray)arg.InnerValue).ToList().ForEach(o => result += (decimal)o);
                }
                else
                {
                    if (!arg.AsDecimal().HasValue)
                        return ExcelValue.VALUE;
                    result += arg.AsDecimal().Value;
                }
            }
            return new ExcelValue.DecimalValue(result, language);
        }
    }
}
