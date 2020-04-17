using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DocumentCreator.ExcelFormulaParser
{
    public partial class Functions
    {
        public ExcelValue CONCATENATE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var parts = args.Select(a => a.ToString(scope.OutLanguage));
            var result = string.Join(string.Empty, parts);
            return new ExcelValue.TextValue(result, scope.OutLanguage);
        }

        public ExcelValue EXACT(List<ExcelValue> args, ExpressionScope scope)
        {
            return ExcelValue.CreateBoolean("=", args[0], args[1], false);
        }

        public ExcelValue FIND(List<ExcelValue> args, ExpressionScope scope)
        {
            var arg3 = (args.Count > 2 && args[2].AsDecimal().HasValue) ? (int)args[2].AsDecimal().Value : 0;
            var result = scope.OutLanguage.IndexOf(args[1].Text, args[0].Text, arg3);
            if (result == null)
                return ExcelValue.NA;
            if (result <= 0)
                return ExcelValue.VALUE;
            return new ExcelValue.DecimalValue(result.Value, scope.OutLanguage);
        }

        public ExcelValue FIXED(List<ExcelValue> args, ExpressionScope scope)
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
            return new ExcelValue.DecimalValue(num, scope.OutLanguage, decimals, !noComma);
        }

        public ExcelValue LEFT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int numChars)) return ExcelValue.VALUE;
            var length = Math.Min(numChars, text.Length);
            return new ExcelValue.TextValue(text.Substring(0, length), scope.OutLanguage);
            
        }

        public ExcelValue RIGHT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int numChars)) return ExcelValue.VALUE;
            var start = Math.Max(0, text.Length - numChars);
            return new ExcelValue.TextValue(text.Substring(start), scope.OutLanguage);
        }

        public ExcelValue MID(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.VALUE;
            if (args.NotInteger(1, 1, out int start)) return ExcelValue.VALUE; 
            if (args.NotInteger(2, 1, out int numChars)) return ExcelValue.VALUE;
            var length = Math.Min(numChars, text.Length - start + 1);
            return new ExcelValue.TextValue(text.Substring(start - 1, length), scope.OutLanguage);
        }

        public ExcelValue LEN(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.DecimalValue(args[0].Text.Length, scope.OutLanguage);
        }

        public ExcelValue LOWER(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(scope.OutLanguage.ToLower(args[0].Text), scope.OutLanguage);
        }

        public ExcelValue PROPER(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(scope.OutLanguage.ToProper(args[0].Text), scope.OutLanguage);
        }

        public ExcelValue REPLACE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.NA;
            if (args.NotPosInteger(1, null, out int startNum)) return ExcelValue.NA;
            if (args.NotInteger(2, null, out int numChars)) return ExcelValue.NA;
            if (args.NotText(3, null, scope.OutLanguage, out string newText)) return ExcelValue.NA;
            --startNum;
            var result = startNum < text.Length ? text.Remove(startNum, numChars) : text;
            result = startNum < text.Length ? result.Insert(startNum, newText) : result + newText;
            return new ExcelValue.TextValue(result, scope.OutLanguage);
        }

        public ExcelValue SEARCH(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string findText)) return ExcelValue.NA;
            if (args.NotText(1, null, scope.OutLanguage, out string withinText)) return ExcelValue.NA;
            if (args.NotPosInteger(2, 1, out int startNum)) return ExcelValue.NA;
            --startNum;
            var pos = withinText.IndexOf(findText, startNum);
            if (pos == -1) return ExcelValue.NA;
            ++pos;
            return new ExcelValue.DecimalValue(pos, scope.OutLanguage);
        }

        public ExcelValue SUBSTITUTE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.NA;
            if (args.NotText(1, null, scope.OutLanguage, out string oldText)) return ExcelValue.NA;
            if (args.NotText(2, null, scope.OutLanguage, out string newText)) return ExcelValue.NA;
            if (args.NotPosInteger(3, int.MaxValue, out int instanceNum)) return ExcelValue.NA;
            if (oldText.Length == 0) return args[0];
            string result;
            if (instanceNum == int.MaxValue)
            {
                result = scope.OutLanguage.Replace(text, oldText, newText, false);
            }
            else
            {
                var counter = 0;
                var pos = -1;
                while ((pos = text.IndexOf(oldText, pos + 1)) != -1)
                {
                    ++counter;
                    if (counter == instanceNum) break;
                }
                if (counter != instanceNum) return args[0];
                result = text.Remove(pos, oldText.Length).Insert(pos, newText);
            }
            return new ExcelValue.TextValue(result, scope.OutLanguage);
        }

        public ExcelValue T(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args[0] is ExcelValue.ErrorValue) return ExcelValue.NA;
            if (args[0] is ExcelValue.TextValue /*|| args[0] is ExcelValue.JsonTextValue*/) return args[0];
            return scope.OutLanguage.EmptyText;
        }

        public ExcelValue TEXT(List<ExcelValue> args, ExpressionScope scope)
        {
            throw new NotSupportedException();
        }

        public ExcelValue TRIM(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string text)) return ExcelValue.NA;
            return new ExcelValue.TextValue(text.Trim(), scope.OutLanguage);
        }

        public ExcelValue UPPER(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            return new ExcelValue.TextValue(scope.OutLanguage.ToUpper(args[0].Text), scope.OutLanguage);
        }

        public ExcelValue VALUE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.ContainErrorValues()) return ExcelValue.NA;
            var textArg = args[0];
            if (textArg is ExcelValue.BooleanValue) return ExcelValue.VALUE;
            if (textArg is ExcelValue.DecimalValue) return args[0];
            if (textArg is ExcelValue.TextValue)
            {
                try
                {
                    var value = scope.OutLanguage.ToDecimal(textArg.Text);
                    return new ExcelValue.DecimalValue(value, scope.OutLanguage);
                }
                catch
                {
                    return ExcelValue.VALUE;
                }
            }
            throw new NotImplementedException();
        }
    }
}
