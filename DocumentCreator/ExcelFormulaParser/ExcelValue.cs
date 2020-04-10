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
                _ => throw new InvalidOperationException($"ExcelValue.Create: invalid subtype {token.Subtype}"),
            };
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

        protected abstract bool? AsBoolean();
        public abstract string ToString(Language language);

        #endregion

        #region Private classes
        private class ErrorValue : ExcelValue
        {
            public ErrorValue(string text) : base(null, text, Language.Invariant)
            {
            }

            public override string ToString(Language language) { return Text; }

            protected override bool? AsBoolean() { return null; }
        }

        private class TextValue : ExcelValue
        {
            public TextValue(string text, Language language) : base(text, text, language)
            {
            }
            protected override bool? AsBoolean() { return null; }
            public override string ToString(Language language) { return Text; }
        }

        private class JsonTextValue : ExcelValue
        {
            public JsonTextValue(JToken token, string text, Language language) : base(token, text, language)
            {
            }
            protected override bool? AsBoolean() { return null; }
            public override string ToString(Language language) { return Text; }
        }

        private class BooleanValue : ExcelValue
        {
            public BooleanValue(bool value) : base(value, value ? "TRUE" : "FALSE", Language.Invariant)
            {
            }
            protected override bool? AsBoolean() { return (bool)InnerValue; }
            public override string ToString(Language language) { return Text; }
        }

        private class DecimalValue : ExcelValue
        {
            public DecimalValue(decimal value, Language language) : base(value, language.ToString(value), language)
            {
            }
            protected override bool? AsBoolean() { return ((decimal)InnerValue) != 0M; }
            public override string ToString(Language language) { return language.ToString((decimal)InnerValue); }
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

        #region Functions

        public static ExcelValue EvaluateFunction(string name, List<ExcelValue> args, Language language, Dictionary<string, JToken> sources)
        {
            switch (name)
            {
                // EXCEL TEXT FUNCTIONS
                case "LEN":
                    if (args[0] == NA) return NA;
                    return new DecimalValue(args[0].Text.Length, language);
                case "LOWER":
                    if (args[0] == NA) return NA;
                    return new TextValue(language.ToLower(args[0].Text), language);
                case "PROPER":
                    if (args[0] == NA) return NA;
                    return new TextValue(language.ToProper(args[0].Text), language);
                case "UPPER":
                    if (args[0] == NA) return NA;
                    return new TextValue(language.ToUpper(args[0].Text), language);

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
                    return new TextValue(DateTime.Today.ToString("d/M/yyyy"), language);
                case "SOURCE":
                    return Create(sources[args[0].Text.ToString()]?.SelectToken(args[1].Text.ToString()), language);
                case "RQD":
                    return Create(sources["RQ"]?["RequestData"]?[args[0].Text], language);
                case "RQL":
                    return Create(sources["RQ"]?["LogHeader"]?[args[0].Text], language);
                case "RQR":
                    return Create(sources["#ROW#"]?[args[0].Text], language);
                case "CONTENT":
                    return new TextValue((args[0].AsBoolean() ?? false) ? "#SHOW_CONTENT#" : "#HIDE_CONTENT#", language);
                default: throw new InvalidOperationException($"Unknown function name: {name}");
            }
        }

        #endregion
    }
}
