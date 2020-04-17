using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue SYSDATE(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return new ExcelValue.TextValue(DateTime.Today.ToString("d/M/yyyy"), language);
        }
        public ExcelValue SOURCE(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return ExcelValue.Create(sources[args[0].Text.ToString()]?.SelectToken(args[1].Text.ToString()), language);
        }
        public ExcelValue RQD(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return ExcelValue.Create(sources["RQ"]?["RequestData"]?[args[0].Text], language);
        }
        public ExcelValue RQL(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return ExcelValue.Create(sources["RQ"]?["LogHeader"]?[args[0].Text], language);
        }
        public ExcelValue RQR(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return ExcelValue.Create(sources["#ROW#"]?[args[0].Text], language);
        }
        public ExcelValue CONTENT(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return new ExcelValue.TextValue((args[0].AsBoolean() ?? false) ? "#SHOW_CONTENT#" : "#HIDE_CONTENT#", language);
        }

        public ExcelValue MAPVALUE(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            var value = ExcelValue.Create(sources[args[0].Text]?.SelectToken(args[1].Text), language);
            if (args.Count > 2 && !(args[2] is ExcelValue.NullValue))
            {
                var args2 = new List<ExcelValue> { args[2], value };
                value = MAPVALUE(args2, language, sources);
            }
            return value;
        }

        public ExcelValue MAPITEM(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            var source = args[0].InnerValue as JArray;
            var items = source.Select(o => o.SelectToken(args[1].Text));
            var result = new JArray(items);
            return ExcelValue.Create(result, language);
        }

        public ExcelValue GETITEM(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            var index = args.Count > 3 ? ((int)args[3].AsDecimal().Value) - 1 : 0;

            var source = args[0].InnerValue as JArray;
            var result = ExcelValue.Create(source[index].SelectToken(args[1].Text), language);
            if (args.Count > 2 && !(args[2] is ExcelValue.NullValue))
            {
                var args2 = new List<ExcelValue> { args[2], result };
                result = MAPVALUE(args2, language, sources);
            }
            return result;
        }
        public ExcelValue GETLIST(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            var source = args[0].InnerValue as JArray;
            var items = source.Select(o => o.SelectToken(args[1].Text));
            var result = new JArray(items);
            return ExcelValue.Create(result, language);
        }
    }
}
