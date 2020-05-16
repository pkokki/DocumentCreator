using JsonExcelExpressions.Eval;
using System;
using System.Globalization;

namespace JsonExcelExpressions.Lang
{
    internal class LanguageEl : Language
    {
        private static readonly char[] UpperAccent = new[] { 'Ά', 'Έ', 'Ή', 'Ί', 'Ϊ', 'Ό', 'Ύ', 'Ϋ', 'Ώ' };
        private static readonly char[] UpperNoAccent = new[] { 'Α', 'Ε', 'Η', 'Ι', 'Ι', 'Ο', 'Υ', 'Υ', 'Ω' };
        //private static readonly char[] LowerAccent = new[]      { 'ά', 'έ', 'ή', 'ί', 'ϊ', 'ό', 'ύ', 'ϋ', 'ώ' };
        //private static readonly char[] LowerNoAccent = new[]    { 'α', 'ε', 'η', 'ι', 'ι', 'ο', 'υ', 'υ', 'ω' };

        public LanguageEl(CultureInfo culture) : base(culture)
        {
        }

        public override string ToLower(string text)
        {
            if (text == null)
                return text;
            var value = base.ToLower(text).Replace("σ ", "ς");
            if (text.Length > 1)
            {
                if (value.EndsWith("σ"))
                    value = value.Remove(value.Length - 1) + "ς";
            }
            return value;
        }

        public override string ToUpper(string text)
        {
            if (text == null)
                return text;
            return ReplaceLetters(base.ToUpper(text), UpperAccent, UpperNoAccent).Replace('ς', 'Σ');
        }

        public override string ToString(DateTime value, ExpressionFormat info)
        {
            // When running unit tests on github we receive "π.μ." instead of "πμ"
            var text = base.ToString(value, info);
            return text?.Replace("π.μ.", "πμ").Replace("μ.μ.", "μμ");
        }

    }
}
