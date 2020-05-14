using JsonExcelExpressions.Eval;
using System;
using System.Globalization;
using System.Text;

namespace JsonExcelExpressions.Lang
{
    internal class Language
    {
        private readonly CultureInfo culture;


        public static Language Invariant
        {
            get
            {
                return new LanguageInvariant(CultureInfo.InvariantCulture);
            }
        }

        public static Language Create(CultureInfo culture)
        {
            if (culture.Name == "el" || culture.Parent?.Name == "el")
                return new LanguageEl(culture);
            else
                return new Language(culture);
        }
        protected Language(CultureInfo culture)
        {
            this.culture = culture;
            EmptyText = new ExcelValue.TextValue(string.Empty, this);
        }

        internal ExcelValue.TextValue EmptyText { get; }
        internal NumberFormatInfo NumberFormat => culture.NumberFormat;

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

        public virtual string ToString(DateTime value, ExpressionFormat info, bool isTime)
        {
            var format = info.GetFormat(isTime ? ExpressionFormat.ShortTimePattern : ExpressionFormat.ShortDatePattern);
            if (format.NeedsDate)
                return string.Format(culture, format.Format, value);
            return string.Format(culture, format.Format, ExcelValue.DateValue.ToSerial(value));
        }

        public string ToString(decimal value, ExpressionFormat info = null)
        {
            info ??= ExpressionFormat.General;
            var format = info.GetFormat(null);
            if (format.NeedsDate)
                return string.Format(culture, format.Format, ExcelValue.DateValue.FromSerial(value));
            return string.Format(culture, format.Format, value);
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
