using JsonExcelExpressions.Lang;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonExcelExpressions.Eval
{
    internal static class ExcelCriteria
    {
        public static Func<ExcelValue, bool> Resolve(ExcelValue criteria, Language language)
        {
            Func<ExcelValue, bool> predicate = null;
            if (criteria is ExcelValue.TextValue)
            {
                var text = criteria.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    if (TryResolveLogical(text, language, out predicate))
                        return predicate;
                    if (IsRegex(text, out string pattern))
                        return v => Regex.IsMatch(v.Text, pattern, RegexOptions.IgnoreCase);
                    predicate = v => v.Text == text;
                }
            }
            if (predicate == null && criteria.InnerValue != null)
                predicate = v => criteria.InnerValue.Equals(v.InnerValue);
            return predicate;
        }

        private static bool TryResolveLogical(string text, Language language, out Func<ExcelValue, bool> predicate)
        {
            predicate = null;
            text = text.Trim();
            if (text.Length > 1)
            {
                if (text[0] == '=' && language.ToDecimal(text.Substring(1), out double value))
                    predicate = v => value.IsEqual(v.AsDecimal());
                else if (text[0] == '>' && language.ToDecimal(text.Substring(1), out value))
                    predicate = v => v.AsDecimal() > value;
                else if (text[0] == '<' && language.ToDecimal(text.Substring(1), out value))
                    predicate = v => v.AsDecimal() < value;
                else if (text.StartsWith("<>") && language.ToDecimal(text.Substring(2), out value))
                    predicate = v => v.AsDecimal() != value;
                else if (text.StartsWith(">=") && language.ToDecimal(text.Substring(2), out value))
                    predicate = v => v.AsDecimal() >= value;
                else if (text.StartsWith("<=") && language.ToDecimal(text.Substring(2), out value))
                    predicate = v => v.AsDecimal() <= value;
            }
            return predicate != null;
        }

        public static bool IsRegex(string text, out string pattern)
        {
            char prev = (char)0;
            var sb = new StringBuilder();
            var isRegex = false;
            foreach (var ch in text)
            {
                if (ch == '*' || ch == '?')
                {
                    if (prev == '~')
                    {
                        sb.Append('\\');
                        sb.Append(ch);
                    }
                    else
                    {
                        isRegex = true;
                        sb.Append(".");
                        sb.Append(ch == '*' ? '*' : '+');
                    }
                }
                else if (ch == '~')
                {
                    // Do nothing
                }
                else
                {
                    sb.Append(ch);
                }
                prev = ch;
            }
            pattern = $"^{sb}$";
            return isRegex;
        }

    }
}
