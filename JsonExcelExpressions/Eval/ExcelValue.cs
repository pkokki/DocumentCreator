using JsonExcelExpressions.Lang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    public abstract class ExcelValue
    {
        #region Singletons

        public static readonly ExcelValue NA = new ErrorValue("#N/A");
        public static readonly ExcelValue VALUE = new ErrorValue("#VALUE!");
        public static readonly ExcelValue DIV0 = new ErrorValue("#DIV/0!");
        public static readonly ExcelValue TRUE = new BooleanValue(true);
        public static readonly ExcelValue FALSE = new BooleanValue(false);
        public static readonly ExcelValue NULL = new NullValue();

        #endregion

        #region Factories

        internal static ExcelValue Create(JToken token, Language language)
        {
            switch (token.Type)
            {
                case JTokenType.Object: return new JsonObjectValue((JObject)token, language);
                case JTokenType.Array: return new ArrayValue((JArray)token, language);
                case JTokenType.Boolean: return new BooleanValue((bool)token);
                case JTokenType.Integer:
                case JTokenType.Float:
                    return new DecimalValue((decimal)token, language);
                default: return new TextValue(token.ToString(), language);
            }
        }

        internal static ExcelValue Create(ExcelFormulaToken token, ExpressionScope scope)
        {
            switch(token.Subtype)
            {
                case ExcelFormulaTokenSubtype.Text: return new TextValue(token.Value, scope.OutLanguage);
                case ExcelFormulaTokenSubtype.Number: return new DecimalValue(decimal.Parse(token.Value, CultureInfo.InvariantCulture), scope.OutLanguage);
                case ExcelFormulaTokenSubtype.Logical: return new BooleanValue(bool.Parse(token.Value));
                case ExcelFormulaTokenSubtype.Range: return new RangeValue(token.Value);
                default: throw new InvalidOperationException($"ExcelValue.Create: invalid subtype {token.Subtype}");
            };
        }

        public static ExcelValue CreateBoolean(string oper, ExcelValue v1, ExcelValue v2, bool ignoreCase = true)
        {
            if (v1 == NA || v2 == NA)
                return NA;
            if (v1 is ErrorValue)
                return v1;
            if (v2 is ErrorValue)
                return v2;
            if (oper == "=")
            {
                if (v1 is BooleanValue)
                    return new BooleanValue(v1 == v2);
                if ((v1 is TextValue && v2 is TextValue)) //*********** || (v1 is JsonTextValue && v2 is JsonTextValue))
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
                if ((v1 is TextValue && v2 is TextValue)) //********** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return new BooleanValue(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) > 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return new BooleanValue(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) > 0);
                return new BooleanValue(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) > 0);

            }
            if (oper == ">=")
            {
                if (v1 is BooleanValue) return new BooleanValue(!(v1 == FALSE && v2 == TRUE));
                if ((v1 is TextValue && v2 is TextValue)) // *************** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return new BooleanValue(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) >= 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return new BooleanValue(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) >= 0);
                return new BooleanValue(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) >= 0);
            }
            throw new InvalidOperationException($"Unhandled comparison {v1?.GetType().Name ?? "NULL"} {oper} {v2?.GetType().Name ?? "NULL"}");
        }

        public virtual ExcelValue ElementAt(int index)
        {
            throw new InvalidOperationException($"{this.GetType().Name} is single-valued. ElementAt is not supported.");
        }

        #endregion

        #region Constructor 

        internal ExcelValue(object value, string text, Language language)
        {
            this.InnerValue = value;
            this.Text = text ?? string.Empty;
            this.Language = language;
        }

        internal ExcelValue(IEnumerable<ExcelValue> value, Language language)
        {
            this.InnerValue = value;
            this.Text = new JArray(value.Select(o => o.Text)).ToString(Formatting.None).Replace("\"", "'");
            this.Language = language;
        }

        #endregion

        #region Properties

        internal Language Language { get; }
        public object InnerValue { get; }
        public string Text { get; }

        #endregion

        #region Methods 

        protected internal abstract bool? AsBoolean();
        protected internal abstract decimal? AsDecimal();
        internal abstract string ToString(Language language, ExpressionFormat info);

        #endregion

        #region Private classes
        internal class ErrorValue : ExcelValue
        {
            public ErrorValue(string text) : base(null, text, Language.Invariant)
            {
            }

            internal override string ToString(Language language, ExpressionFormat info) { return Text; }

            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
        }

        internal class NullValue : ExcelValue
        {
            public NullValue() : base(null, null, Language.Invariant)
            {
            }
            internal override string ToString(Language language, ExpressionFormat info) { return null; }
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
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
        }

        internal class JsonObjectValue : ExcelValue
        {
            public JsonObjectValue(JObject token, Language language) : base(token, "{}", language)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
        }

        internal class ArrayValue : ExcelValue
        {
            private readonly bool? asBoolean;
            private readonly decimal? asDecimal;
            private readonly IEnumerable<ExcelValue> values;

            public ArrayValue(JArray token, Language language)
                : this(token.Select(o => Create(o, language)).ToArray(), language)
            {
            }
            public ArrayValue(IEnumerable<ExcelValue> value, Language language)
                : base(value, language)
            {
                values = value;
                if (values.Any())
                {
                    asBoolean = values.ElementAt(0).AsBoolean();
                    asDecimal = values.ElementAt(0).AsDecimal();
                }
            }

            protected internal override bool? AsBoolean() { return asBoolean; }
            protected internal override decimal? AsDecimal() { return asDecimal; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override ExcelValue ElementAt(int index)
            {
                return values.ElementAt(index);
            }
        }

        internal class SourceReferenceValue : ExcelValue
        {
            public SourceReferenceValue(string sourceName) : base(null, sourceName, Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
        }

        internal class BooleanValue : ExcelValue
        {
            public BooleanValue(bool value) : base(value, value ? "TRUE" : "FALSE", Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return (bool)InnerValue; }
            protected internal override decimal? AsDecimal() { return (bool)InnerValue ? 1M : 0M; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
        }

        internal class RangeValue : ExcelValue
        {
            public RangeValue(string value) : base(value, value, Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override decimal? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
        }

        internal class DateValue : DecimalValue
        {
            private static readonly DateTime BASE = new DateTime(1900, 1, 1);
            public DateValue(int year, int month, int day, Language language, ExpressionFormat format)
                : this(ToSerial(year, month, day), language, format)
            {
            }
            public DateValue(int year, int month, int day, int hours, int minutes, int seconds, Language language, ExpressionFormat format)
                : this(ToSerial(year, month, day, hours, minutes, seconds), language, format)
            {
            }
            public DateValue(decimal serial, Language language, ExpressionFormat format)
                : base(serial, language, ExpressionFormat.General)
            {
                Serial = serial;
                Date = BASE.AddDays((double)serial - 1);
                Format = format;
            }

            public decimal Serial { get; }
            public DateTime Date { get; }
            public ExpressionFormat Format { get; }

            public static decimal ToSerial(DateTime date)
            {
                return ToSerial(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }
            private static decimal ToSerial(int year, int month, int day, int hours = 0, int minutes = 0, int seconds = 0)
            {
                var date = new DateTime(
                    year > 0 ? year : 1900,
                    month > 0 ? month : 1,
                    day > 0 ? day : 1,
                    hours, minutes, seconds).AddDays(-1);
                var serial = date.ToOADate();
                if (serial < 60)
                    serial -= 1;
                return (decimal)serial;
            }
            public static DateTime FromSerial(decimal serial)
            {
                // Excel/Lotus 123 have a bug with 29-02-1900. 1900 is not a
                // leap year, but Excel/Lotus 123 think it is...
                if (serial < 60)
                    serial += 1;
                var date = DateTime.FromOADate((double)serial);
                if (date.Millisecond >= 500)
                    return date.AddSeconds(1);
                return date;
            }

            internal override string ToString(Language language, ExpressionFormat info)
            {
                if (info == null)
                    info = Format;
                return language.ToString(Date, info);
            }
        }

        internal class DecimalValue : ExcelValue
        {
            public DecimalValue(decimal value, Language language, ExpressionFormat format = null)
                : base(value, language.ToString(value, format), language)
            {
            }
            protected internal override bool? AsBoolean() { return ((decimal)InnerValue) != 0M; }
            protected internal override decimal? AsDecimal() { return (decimal)InnerValue; }
            internal override string ToString(Language language, ExpressionFormat info) 
            {
                if (info != null)
                    return language.ToString((decimal)InnerValue, info);
                return Text;
            }
        }

        #endregion

        #region Operators

        public static ExcelValue operator +(ExcelValue a, ExcelValue b)
        {
            var value = a.AsDecimal() + b.AsDecimal();
            if (!value.HasValue)
                return NA;
            if (a is DateValue d)
                return new DateValue(value.Value, d.Language, d.Format);
            return new DecimalValue(value.Value, a.Language);
        }
        public static ExcelValue operator -(ExcelValue a, ExcelValue b)
        {
            var value = a.AsDecimal() - b.AsDecimal();
            if (!value.HasValue)
                return NA;
            if (a is DateValue d)
                return new DateValue(value.Value, a.Language, d.Format);
            return new DecimalValue(value.Value, a.Language);
        }
        public static ExcelValue operator *(ExcelValue a, ExcelValue b)
        {
            var value = a.AsDecimal() * b.AsDecimal();
            if (!value.HasValue)
                return NA;
            if (a is DateValue d)
                return new DateValue(value.Value, a.Language, d.Format);
            return new DecimalValue(value.Value, a.Language);
        }
        public static ExcelValue operator /(ExcelValue a, ExcelValue b)
        {
            var denominator = b.AsDecimal();
            return a / denominator.Value;
        }
        public static ExcelValue operator /(ExcelValue a, decimal denominator)
        {
            if (denominator == 0M)
                return DIV0;
            var value = a.AsDecimal() / denominator;
            if (!value.HasValue)
                return NA;
            if (a is DateValue d)
                return new DateValue(value.Value, a.Language, d.Format);
            return new DecimalValue(value.Value, a.Language);
        }
        public static ExcelValue operator -(ExcelValue a)
        {
            return new DecimalValue(-a.AsDecimal().Value, a.Language);
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
