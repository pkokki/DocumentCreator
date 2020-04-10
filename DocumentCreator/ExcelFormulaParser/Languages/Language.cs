﻿using System;
using System.Linq;
using System.Globalization;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser.Languages
{
    public abstract class Language
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

        protected Language(CultureInfo culture)
        {
            this.culture = culture;
        }

        public string ToString(ExcelValue value)
        {
            return value?.ToString(this);
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

        public decimal ToDecimal(string value)
        {
            return Convert.ToDecimal(value, culture);
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
    }
}