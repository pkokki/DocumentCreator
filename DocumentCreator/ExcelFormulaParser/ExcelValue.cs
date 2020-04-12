using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public abstract class ExcelValue
    {
        #region Singletons

        public static readonly ExcelValue NA = new ErrorValue("#N/A");
        public static readonly ExcelValue VALUE = new ErrorValue("#VALUE!");
        public static readonly ExcelValue DIV0 = new ErrorValue("#DIV/0!");
        public static readonly ExcelValue TRUE = new BooleanValue(true);
        public static readonly ExcelValue FALSE = new BooleanValue(false);

        #endregion

        #region Factories

        public static ExcelValue Create(JToken token, Language language)
        {
            switch (token.Type)
            {
                case JTokenType.Object: return new JsonTextValue(token, "{}", language);
                case JTokenType.Array: return new JsonTextValue(token, "[]", language);
                case JTokenType.Boolean: return new BooleanValue((bool)token);
                case JTokenType.Integer:
                case JTokenType.Float:
                    return new DecimalValue((decimal)token, language);
                default: return new TextValue(token.ToString(), language);
            }
        }

        public static ExcelValue Create(ExcelFormulaToken token, ExpressionContext context)
        {
            return token.Subtype switch
            {
                ExcelFormulaTokenSubtype.Text => new TextValue(token.Value, context.OutputLang),
                ExcelFormulaTokenSubtype.Number => new DecimalValue(context.InputLang.ToDecimal(token.Value), context.OutputLang),
                ExcelFormulaTokenSubtype.Logical => new BooleanValue(context.InputLang.ToBoolean(token.Value)),
                ExcelFormulaTokenSubtype.Range => new RangeValue(token.Value),
                _ => throw new InvalidOperationException($"ExcelValue.Create: invalid subtype {token.Subtype}"),
            };
        }

        public static ExcelValue CreateBoolean(string oper, ExcelValue v1, ExcelValue v2, bool ignoreCase = true)
        {
            if (v1 == NA|| v2 == NA)
                return NA;
            if (v1 is ErrorValue)
                return v1;
            if (v2 is ErrorValue)
                return v2;
            if (oper == "=")
            {
                if (v1 is BooleanValue) 
                    return new BooleanValue(v1 == v2);
                if ((v1 is TextValue && v2 is TextValue) || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return new BooleanValue(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) == 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return new BooleanValue(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) == 0);
                return FALSE;
            }
            if (oper == "<" || oper == "<=")
            {
                var tmp = v1;
                v1 = v2;
                v2 = tmp;
                oper = oper == "<" ? ">=" : ">";
            }
            if (oper == ">")
            {
                if (v1 is BooleanValue) return new BooleanValue(v1 == TRUE && v2 == FALSE);
                if ((v1 is TextValue && v2 is TextValue) || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return new BooleanValue(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) > 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return new BooleanValue(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) > 0);
                return new BooleanValue(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) > 0);

            }
            if (oper == ">=")
            {
                if (v1 is BooleanValue) return new BooleanValue(!(v1 == FALSE && v2 == TRUE));
                if ((v1 is TextValue && v2 is TextValue) || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return new BooleanValue(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) >= 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return new BooleanValue(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) >= 0);
                return new BooleanValue(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) >= 0);
            }
            throw new InvalidOperationException($"Unhandled comparison {v1?.GetType().Name ?? "NULL"} {oper} {v2?.GetType().Name ?? "NULL"}");
        }

        #endregion

        #region Constructor 

        protected ExcelValue(object value, string text, Language language)
        {
            this.InnerValue = value;
            this.Text = text ?? string.Empty;
            this.Language = language;
        }

        #endregion

        #region Properties

        protected Language Language { get; }
        public object InnerValue { get; }
        public string Text { get; }

        #endregion

        #region Methods 

        protected internal abstract bool? AsBoolean();
        protected internal abstract decimal? AsDecimal();
        public abstract string ToString(Language language);

        #endregion

        #region Private classes
        internal class ErrorValue : ExcelValue
        {
            public ErrorValue(string text) : base(null, text, Language.Invariant)
            {
            }

            public override string ToString(Language language) { return Text; }

            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
        }

        internal class TextValue : ExcelValue
        {
            public TextValue(string text, Language language) : base(text, text, language)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() 
            {
                if (decimal.TryParse(Text, out decimal v))
                    return v;
                return null;
            }
            public override string ToString(Language language) { return Text; }
        }

        internal class JsonTextValue : ExcelValue
        {
            public JsonTextValue(JToken token, string text, Language language) : base(token, text, language)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
            public override string ToString(Language language) { return Text; }
        }

        internal class BooleanValue : ExcelValue
        {
            public BooleanValue(bool value) : base(value, value ? "TRUE" : "FALSE", Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return (bool)InnerValue; }
            protected internal override decimal? AsDecimal() { return (bool)InnerValue ? 1M : 0M; }
            public override string ToString(Language language) { return Text; }
        }

        internal class RangeValue : ExcelValue
        {
            public RangeValue(string value) : base(value, value, Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
            public override string ToString(Language language) { return Text; }
        }

        internal class DecimalValue : ExcelValue
        {
            private readonly int? decimals;
            private readonly bool commas;

            public DecimalValue(decimal value, Language language, int? decimals = null, bool commas = false) 
                : base(value, language.ToString(value, decimals, commas), language)
            {
                this.decimals = decimals;
                this.commas = commas;
            }
            protected internal override bool? AsBoolean() { return ((decimal)InnerValue) != 0M; }
            protected internal override decimal? AsDecimal() { return (decimal)InnerValue; }
            public override string ToString(Language language) { return language.ToString((decimal)InnerValue, decimals, commas); }
        }

        #endregion

        #region Operators

        public static ExcelValue operator +(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) + Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Language);
        }
        public static ExcelValue operator -(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) - Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Language);
        }
        public static ExcelValue operator *(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) * Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Language);
        }
        public static ExcelValue operator /(ExcelValue a, ExcelValue b)
        {
            var denominator = Convert.ToDecimal(b.InnerValue);
            return a / denominator;
        }
        public static ExcelValue operator /(ExcelValue a, decimal denominator)
        {
            if (denominator == 0M)
                return DIV0;
            var value = Convert.ToDecimal(a.InnerValue) / denominator;
            return new DecimalValue(value, a.Language);
        }
        public static ExcelValue operator -(ExcelValue a)
        {
            return new DecimalValue(-Convert.ToDecimal(a.InnerValue), a.Language);
        }
        public static ExcelValue operator ^(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(Math.Pow(Convert.ToDouble(a.InnerValue), Convert.ToDouble(b.InnerValue)));
            return new DecimalValue(value, a.Language);
        }
        public static ExcelValue operator &(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToString(a.InnerValue) + Convert.ToString(b.InnerValue);
            return new TextValue(value, a.Language);
        }
        #endregion

    }
}
