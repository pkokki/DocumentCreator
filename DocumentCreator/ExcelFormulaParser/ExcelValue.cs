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

        public static ExcelValue Create(JToken token, CultureInfo culture)
        {
            switch (token.Type)
            {
                case JTokenType.Object: return new JsonTextValue(token, "{}", culture);
                case JTokenType.Array: return new JsonTextValue(token, "[]", culture);
                case JTokenType.Boolean: return new BooleanValue((bool)token);
                case JTokenType.Integer:
                case JTokenType.Float:
                    return new DecimalValue((decimal)token, culture);
                default: return new TextValue(token.ToString(), culture);
            }
        }

        public static ExcelValue Create(ExcelFormulaToken token, CultureInfo culture)
        {
            return token.Subtype switch
            {
                ExcelFormulaTokenSubtype.Text => new TextValue(token.Value, culture),
                ExcelFormulaTokenSubtype.Number => new DecimalValue(Convert.ToDecimal(token.Value, culture), culture),
                ExcelFormulaTokenSubtype.Logical => new BooleanValue(Convert.ToBoolean(token.Value, culture)),
                _ => throw new InvalidOperationException($"ExcelValue.Create: invalid subtype {token.Subtype}"),
            };
        }

        public static ExcelValue Create(ExcelFormulaValue efv, CultureInfo culture)
        {
            if (efv.HasValue)
                return efv.Value;
            return Create(efv.Token, culture);
        }

        public static ExcelValue CreateBoolean(string oper, ExcelValue v1, ExcelValue v2)
        {
            var a = v1.InnerValue;
            var b = v2.InnerValue;
            if (a == null)
                return new BooleanValue(b == null);
            else if (b == null)
                return FALSE;
            else
            {
                // Both not null
                object a1, b1;
                try
                {
                    a1 = a;
                    b1 = Convert.ChangeType(b, a.GetType());
                }
                catch
                {
                    b1 = b;
                    try
                    {
                        a1 = Convert.ChangeType(a, b.GetType());
                    }
                    catch
                    {
                        return ErrorValue.NA;
                    }
                }
                if (!(a1 is IComparable comparable))
                    return NA;

                return oper switch
                {
                    "=" => new BooleanValue(comparable.CompareTo(b1) == 0),
                    ">" => new BooleanValue(comparable.CompareTo(b1) > 0),
                    ">=" => new BooleanValue(comparable.CompareTo(b1) >= 0),
                    "<" => new BooleanValue(comparable.CompareTo(b1) < 0),
                    "<=" => new BooleanValue(comparable.CompareTo(b1) <= 0),
                    _ => throw new InvalidOperationException($"Unknown logical operator: {oper}"),
                };
            }
        }

        #endregion

        #region Constructor 

        protected ExcelValue(object value, string text, CultureInfo culture)
        {
            this.InnerValue = value;
            this.Text = text ?? string.Empty;
            this.Culture = culture;
        }

        #endregion

        #region Properties

        protected CultureInfo Culture { get; }
        public object InnerValue { get; }
        public string Text { get; }

        #endregion

        #region Methods 

        protected abstract bool? AsBoolean();
        public abstract string ToString(CultureInfo culture);

        #endregion

        #region Private classes
        private class ErrorValue : ExcelValue
        {
            public ErrorValue(string text) : base(null, text, CultureInfo.InvariantCulture)
            {
            }

            public override string ToString(CultureInfo culture) { return Text; }

            protected override bool? AsBoolean() { return null; }
        }

        private class TextValue : ExcelValue
        {
            public TextValue(string text, CultureInfo culture) : base(text, text, culture)
            {
            }
            protected override bool? AsBoolean() { return null; }
            public override string ToString(CultureInfo culture) { return Text; }
        }

        private class JsonTextValue : ExcelValue
        {
            public JsonTextValue(JToken token, string text, CultureInfo culture) : base(token, text, culture)
            {
            }
            protected override bool? AsBoolean() { return null; }
            public override string ToString(CultureInfo culture) { return Text; }
        }

        private class BooleanValue : ExcelValue
        {
            public BooleanValue(bool value) : base(value, value ? "TRUE" : "FALSE", CultureInfo.InvariantCulture)
            {
            }
            protected override bool? AsBoolean() { return (bool)InnerValue; }
            public override string ToString(CultureInfo culture) { return Text; }
        }

        private class DecimalValue : ExcelValue
        {
            public DecimalValue(decimal value, CultureInfo culture) : base(value, Convert.ToString(value, culture), culture)
            {
            }
            protected override bool? AsBoolean() { return ((decimal)InnerValue) != 0M; }
            public override string ToString(CultureInfo culture) { return Convert.ToString(InnerValue, culture); }
        }

        #endregion

        #region Operators

        public static ExcelValue operator +(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) + Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Culture);
        }
        public static ExcelValue operator -(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) - Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Culture);
        }
        public static ExcelValue operator *(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(a.InnerValue) * Convert.ToDecimal(b.InnerValue);
            return new DecimalValue(value, a.Culture);
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
            return new DecimalValue(value, a.Culture);
        }
        public static ExcelValue operator -(ExcelValue a)
        {
            return new DecimalValue(-Convert.ToDecimal(a.InnerValue), a.Culture);
        }
        public static ExcelValue operator ^(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToDecimal(Math.Pow(Convert.ToDouble(a.InnerValue), Convert.ToDouble(b.InnerValue)));
            return new DecimalValue(value, a.Culture);
        }
        public static ExcelValue operator &(ExcelValue a, ExcelValue b)
        {
            var value = Convert.ToString(a.InnerValue) + Convert.ToString(b.InnerValue);
            return new TextValue(value, a.Culture);
        }
        #endregion

        #region Functions

        public static ExcelValue EvaluateFunction(string name, List<ExcelValue> args, CultureInfo culture, Dictionary<string, JToken> sources)
        {
            switch (name)
            {
                // EXCEL TEXT FUNCTIONS
                case "LEN":
                    if (args[0] == NA) return NA;
                    return new DecimalValue(args[0].Text.Length, culture);
                case "LOWER":
                    if (args[0] == NA) return NA;
                    return new TextValue(args[0].Text.ToLower(culture), culture);
                case "PROPER":
                    if (args[0] == NA) return NA;
                    return new TextValue(culture.TextInfo.ToTitleCase(args[0].Text), culture);
                case "UPPER":
                    if (args[0] == NA) return NA;
                    return new TextValue(args[0].Text.ToUpper(culture), culture);

                // EXCEL LOGICAL FUNCTIONS
                case "AND":
                    if (args.Any(a => !a.AsBoolean().HasValue)) return NA;
                    return new BooleanValue(args.All(o => o.AsBoolean().Value));
                case "IF":
                    if (!args[0].AsBoolean().HasValue) return NA;
                    return args[0].AsBoolean().Value ? args[1] : args[2];
                case "IFERROR":
                    return args[0] is ErrorValue ? args[1] : args[0];
                case "IFNA":
                    return NA.Equals(args[0]) ? args[1] : args[0];
                case "NOT":
                    if (args[0] is TextValue) return VALUE;
                    if (!args[0].AsBoolean().HasValue) return NA;
                    return new BooleanValue(!args[0].AsBoolean().Value);
                case "OR":
                    if (args.Any(a => !a.AsBoolean().HasValue)) return NA;
                    return new BooleanValue(args.Any(o => o.AsBoolean().Value));
                case "XOR":
                    if (args.Any(a => !a.AsBoolean().HasValue)) return NA;
                    return new BooleanValue(args.Select(o => o.AsBoolean().Value).Aggregate((a, b) => a ^ b));

                // EXCEL ****** FUNCTIONS
                case "NA": return NA;

                // CUSTOM UDF FUNCTIONS
                case "SYSDATE":
                    return new TextValue(DateTime.Today.ToString("d/M/yyyy"), culture);
                case "SOURCE":
                    return Create(sources[args[0].Text.ToString()]?.SelectToken(args[1].Text.ToString()), culture);
                case "RQD":
                    return Create(sources["RQ"]?["RequestData"]?[args[0].Text], culture);
                case "RQL":
                    return Create(sources["RQ"]?["LogHeader"]?[args[0].Text], culture);
                case "RQR":
                    return Create(sources["#ROW#"]?[args[0].Text], culture);
                case "CONTENT":
                    return new TextValue((args[0].AsBoolean() ?? false) ? "#SHOW_CONTENT#" : "#HIDE_CONTENT#", culture);
                default: throw new InvalidOperationException($"Unknown function name: {name}");
            }
        }

        #endregion
    }
}
