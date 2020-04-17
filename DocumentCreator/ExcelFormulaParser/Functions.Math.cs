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
            var result = 0M;
            foreach (var arg in args)
            {
                if (arg is ExcelValue.JsonTextValue && arg.InnerValue is JArray)
                {
                    ((JArray)arg.InnerValue).ToList().ForEach(o => result += (decimal)o);
                }
                else
                {
                    result += arg.AsDecimal().Value;
                }
            }
            return new ExcelValue.DecimalValue(result, language);
        }
    }
}
