using JsonExcelExpressions.Eval;
using System;
using System.Globalization;
using System.Text;

namespace JsonExcelExpressions.Lang
{
    internal class Language
    {
        private const double MIN_NUMBER = (double)decimal.MinValue;
        private const double MAX_NUMBER = (double)decimal.MaxValue;
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

        public virtual string ToString(DateTime value, ExpressionFormat info)
        {
            var format = info.GetFormat(ExpressionFormat.ShortDatePattern);
            if (format.NeedsDate)
                return string.Format(culture, format.Format, value);
            // Should start from beginning in order to use the overriden methods
            var serial = ExcelValue.ToDateSerial(value);
            if (serial.HasValue)
                return ToString(serial.Value, info);
            return ExcelValue.NA.ToString();
        }

        public string ToString(double value, ExpressionFormat info = null)
        {
            string text;
            if (value < MIN_NUMBER || value > MAX_NUMBER)
            {
                text = ExcelValue.VALUE.ToString();
            }
            else
            {
                info ??= ExpressionFormat.General;
                var format = info.GetFormat(null);
                if (format.NeedsDate)
                {
                    var date = ExcelValue.FromDateSerial(value);
                    // Should start from beginning in order to use the overriden methods
                    if (date.HasValue)
                        text = ToString(date.Value, info);
                    else
                       text = ExcelValue.NA.ToString();
                }
                else
                {
                    text = string.Format(culture, format.Format, Convert.ToDecimal(value));
                }
            }
            return text;
        }

        public double ToDecimal(string value)
        {
            var result = double.Parse(value, NumberStyles.Number, culture);
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

        public bool TryParseDecimal(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Any, culture, out result);
        }

        public bool TryParseDateTime(string value, out DateTime result)
        {
            return DateTime.TryParse(value, culture, DateTimeStyles.None, out result);
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
