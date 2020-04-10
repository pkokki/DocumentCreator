using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue EXACT(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            return ExcelValue.CreateBoolean("=", args[0], args[1], false);
        }

        public ExcelValue FIND(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            var arg3 = (args.Count > 2 && args[2].AsDecimal().HasValue) ? (int)args[2].AsDecimal().Value : 0;
            var result = language.IndexOf(args[1].Text, args[0].Text, arg3);
            if (result == null)
                return ExcelValue.NA;
            if (result <= 0)
                return ExcelValue.VALUE;
            return new ExcelValue.DecimalValue(result.Value, language);
        }

        public ExcelValue FIXED(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotDecimal(0, null, out decimal num)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 2, out int decimals)) return ExcelValue.VALUE;
            if (args.NotBoolean(2, false, out bool noComma)) return ExcelValue.VALUE;
            if (decimals < 0)
            {
                var factor = Convert.ToDecimal(Math.Pow(10, -decimals));
                num = Math.Round(num / factor, 0) * factor;
                decimals = 0;
            }
            return new ExcelValue.DecimalValue(num, language, decimals, !noComma);
        }

        public ExcelValue LEFT(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, language, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int numChars)) return ExcelValue.VALUE;
            var length = Math.Min(numChars, text.Length);
            return new ExcelValue.TextValue(text.Substring(0, length), language);
            
        }

        public ExcelValue RIGHT(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, language, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int numChars)) return ExcelValue.VALUE;
            var start = Math.Max(0, text.Length - numChars);
            return new ExcelValue.TextValue(text.Substring(start), language);
        }

        public ExcelValue MID(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, language, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int start)) return ExcelValue.VALUE; 
            if (args.NotInteger(2, 1, out int numChars)) return ExcelValue.VALUE;
            var length = Math.Min(numChars, text.Length - start + 1);
            return new ExcelValue.TextValue(text.Substring(start - 1, length), language);
        }

        public ExcelValue LEN(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.DecimalValue(args[0].Text.Length, language);
        }

        public ExcelValue LOWER(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(language.ToLower(args[0].Text), language);
        }

        public ExcelValue PROPER(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(language.ToProper(args[0].Text), language);
        }

        public ExcelValue UPPER(List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(language.ToUpper(args[0].Text), language);
        }
    }
}
