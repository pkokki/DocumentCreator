using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser.Languages
{
    public class LanguageElGr : Language
    {
        private static readonly char[] UpperAccent = new[]      { 'Ά', 'Έ', 'Ή', 'Ί', 'Ϊ', 'Ό', 'Ύ', 'Ϋ', 'Ώ' };
        private static readonly char[] UpperNoAccent = new[]    { 'Α', 'Ε', 'Η', 'Ι', 'Ι', 'Ο', 'Υ', 'Υ', 'Ω' };
        //private static readonly char[] LowerAccent = new[]      { 'ά', 'έ', 'ή', 'ί', 'ϊ', 'ό', 'ύ', 'ϋ', 'ώ' };
        //private static readonly char[] LowerNoAccent = new[]    { 'α', 'ε', 'η', 'ι', 'ι', 'ο', 'υ', 'υ', 'ω' };

        public LanguageElGr(CultureInfo culture) : base(culture)
        {
        }

        public override string ToLower(string text)
        {
            if (text == null)
                return text;
            var value = base.ToLower(text).Replace("σ ", "ς");
            if (text.Length > 1)
            {
                if (value[^1] == 'σ')
                    value = value.Remove(value.Length - 1) + 'ς';
            }
            return value;
        }

        public override string ToUpper(string text)
        {
            if (text == null)
                return text;
            return ReplaceLetters(base.ToUpper(text), UpperAccent, UpperNoAccent).Replace('ς', 'Σ');
        }
    }
}
