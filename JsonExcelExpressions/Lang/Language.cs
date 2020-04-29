using JsonExcelExpressions.Eval;
using System;
using System.Globalization;
using System.Text;

namespace JsonExcelExpressions.Lang
{
    public class Language
    {
        private readonly CultureInfo culture;


        public static Language Invariant
        {
            get
            {
                return new LanguageInvariant(CultureInfo.InvariantCulture);
            }
        }
        public static Language ElGr
        {
            get
            {
                return new LanguageElGr(CultureInfo.GetCultureInfo("el-GR"));
            }
        }

        public Language(CultureInfo culture)
        {
            this.culture = culture;
            EmptyText = new ExcelValue.TextValue(string.Empty, this);
        }

        internal ExcelValue.TextValue EmptyText { get; }

        protected string ReplaceLetters(string text, char[] toReplace, char[] replacements)
        {
            var builder = new StringBuilder(text);
            for (int i = 0; i < builder.Length; ++i)
            {
                var currentChar = builder[i];
                var index = Array.IndexOf(toReplace, currentChar);
                if (index >= 0)
                    builder[i] = replacements[index];
            }
            return builder.ToString();
        }

        public string ToString(ExcelValue value)
        {
            return value?.ToString(this);
        }

        public virtual string ToString(DateTime value, string format = null)
        {
            format ??= culture.DateTimeFormat.ShortDatePattern;
            return value.ToString(format, culture);
        }
        public virtual string ToTimeString(DateTime value, string format = null)
        {
            format ??= culture.DateTimeFormat.ShortTimePattern;
            return value.ToString(format, culture);
        }

        public string ToString(decimal value, int? decimals = null, bool commas = false)
        {
            string format = null;
            if (commas)
                format = $"N{decimals}";
            else if (decimals.HasValue)
                format = $"F{decimals.Value}";
            return value.ToString(format, culture);
        }

        public string ToString(decimal value, string format)
        {
            var translatedFormat = TranslateFormat(format, culture);
            return value.ToString(translatedFormat, culture);
        }

        private string TranslateFormat(string format, CultureInfo culture)
        {
            var sb = new StringBuilder();
            foreach (var ch1 in format)
            {
                var ch2 = ch1;
                var nf = culture.NumberFormat;
                if (nf.NumberDecimalSeparator[0] == ch1)
                    ch2 = '.';
                else if (nf.NumberGroupSeparator[0] == ch1)
                    ch2 = ',';
                sb.Append(ch2);
            }
            return sb.ToString();
        }

        public decimal ToDecimal(string value)
        {
            var result = decimal.Parse(value, NumberStyles.Number, culture);
            // Check for wrong thousands separators
            if (value != ToString(result))
                throw new ArgumentException($"Wrong decimal text: '{value}' -> '{result}'");
            return result;
        }

        public bool ToBoolean(string value)
        {
            return Convert.ToBoolean(value, culture);
        }

        public virtual string ToLower(string text)
        {
            return text?.ToLower(culture);
        }
        public virtual string ToUpper(string text)
        {
            return text?.ToUpper(culture);
        }
        public string ToProper(string text)
        {
            return culture.TextInfo.ToTitleCase(text);
        }

        public bool TryParseDecimal(string value, out decimal result)
        {
            return decimal.TryParse(value, NumberStyles.Any, culture, out result);
        }

        public int? IndexOf(string target, string text, int startIndex)
        {
            if (target != null && text != null && startIndex >= 0)
                return target.IndexOf(text, startIndex, StringComparison.CurrentCulture) + 1;
            return null;
        }

        public string Replace(string text, string oldText, string newText, bool ignoreCase)
        {
            return text.Replace(oldText, newText, ignoreCase, culture);
        }
    }
}
