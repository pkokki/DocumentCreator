using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
