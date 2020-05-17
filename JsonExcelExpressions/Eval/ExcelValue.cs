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
        public static readonly ExcelValue TRUE = BooleanValue._TRUE;
        public static readonly ExcelValue FALSE = BooleanValue._FALSE;
        public static readonly ExcelValue NULL = new NullValue();
        public static readonly ExcelValue ZERO = new DecimalValue(0, Language.Invariant);
        public static readonly ExcelValue ONE = new DecimalValue(1, Language.Invariant);
        public static readonly ExcelValue MINUS_ONE = new DecimalValue(-1, Language.Invariant);
        public static readonly ExcelValue HUNDRED = new DecimalValue(100, Language.Invariant);

        #endregion

        #region Static methods
        public static decimal? ToDateSerial(DateTime date)
        {
            return ToDateSerial(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
        public static decimal? ToDateSerial(int year, int month, int day, int hours = 0, int minutes = 0, int seconds = 0)
        {
            if (year > 0 && month > 0 && day > 0)
            {
                var date = new DateTime(year, month, day, hours, minutes, seconds);
                var serial = date.ToOADate() - 1;
                if (serial <= 0)
                    return null;
                // Excel/Lotus 123 have a bug with 29-02-1900. 1900 is not a
                // leap year, but Excel/Lotus 123 think it is...
                if (serial >= 60)
                    ++serial;
                return (decimal)serial;
            }
            else
            {
                var serial = Math.Round((hours * 60 * 60 + minutes * 60 + seconds) / 86400M, 10);
                if (serial < 0)
                    return null;
                return serial;
            }
        }

        public static DateTime? FromDateSerial(decimal serial)
        {
            if (serial > 0 && serial < 1)
            {
                var seconds = Math.Round(serial * 86400M);
                return DateValue.BASE.AddSeconds((double)seconds);
            }
            else
            {
                // Excel/Lotus 123 have a bug with 29-02-1900. 1900 is not a
                // leap year, but Excel/Lotus 123 think it is...
                if (serial < 60)
                    ++serial;
                var date = DateTime.FromOADate((double)serial);
                if (date < DateValue.BASE)
                    return null;
                if (date.Millisecond >= 500)
                    return date.AddSeconds(1);
                return date;
            }
        }
        #endregion

        #region Factories

        internal static ExcelValue Create(JToken token, Language language)
        {
            switch (token.Type)
            {
                case JTokenType.Object: return new JsonObjectValue((JObject)token, language);
                case JTokenType.Array: return new ArrayValue((JArray)token, language);
                case JTokenType.Boolean: return BooleanValue.Create((bool)token);
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
                case ExcelFormulaTokenSubtype.Logical: return BooleanValue.Create(bool.Parse(token.Value));
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
                    return BooleanValue.Create(v1 == v2);
                if ((v1 is TextValue && v2 is TextValue)) //*********** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return BooleanValue.Create(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) == 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return BooleanValue.Create(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) == 0);
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
                if (v1 is BooleanValue) return BooleanValue.Create(v1 == TRUE && v2 == FALSE);
                if ((v1 is TextValue && v2 is TextValue)) //********** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return BooleanValue.Create(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) > 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return BooleanValue.Create(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) > 0);
                return BooleanValue.Create(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) > 0);

            }
            if (oper == ">=")
            {
                if (v1 is BooleanValue) return BooleanValue.Create(!(v1 == FALSE && v2 == TRUE));
                if ((v1 is TextValue && v2 is TextValue)) // *************** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return BooleanValue.Create(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) >= 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return BooleanValue.Create(decimal.Compare((decimal)v1.InnerValue, (decimal)v2.InnerValue) >= 0);
                return BooleanValue.Create(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) >= 0);
            }
            throw new InvalidOperationException($"Unhandled comparison {v1?.GetType().Name ?? "NULL"} {oper} {v2?.GetType().Name ?? "NULL"}");
        }

        internal static ExcelValue CreateDateValue(int year, int month, int day, Language language, ExpressionFormat format)
        {
            return CreateDateValue(year, month, day, 0, 0, 0, language, format);
        }
        internal static ExcelValue CreateDateValue(int year, int month, int day, int hours, int minutes, int seconds, Language language, ExpressionFormat format)
        {
            var serial = ToDateSerial(year, month, day, hours, minutes, seconds);
            if (serial.HasValue)
                return new DateValue(serial.Value, language, format);
            return NA;
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

        public virtual ExcelValue ElementAt(int index)
        {
            throw new InvalidOperationException($"{this.GetType().Name} is single-valued. ElementAt is not supported.");
        }

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
            internal override string ToString(Language language, ExpressionFormat info) { return string.Empty; }
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
                if (Language.TryParseDecimal(Text, out decimal v))
                    return v;
                if (Language.TryParseDateTime(Text, out DateTime d))
                    return DateValue.ToDateSerial(d);
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

            public IEnumerable<ExcelValue> Values => values;

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
            public static readonly ExcelValue _TRUE = new BooleanValue(true);
            public static readonly ExcelValue _FALSE = new BooleanValue(false);
            internal static ExcelValue Create(bool value)
            {
                return value ? _TRUE : _FALSE;
            }

            private BooleanValue(bool value) : base(value, value ? "TRUE" : "FALSE", Language.Invariant)
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
            internal static readonly DateTime BASE = new DateTime(1900, 1, 1);
            
            public DateValue(decimal serial, Language language, ExpressionFormat format)
                : base(serial, language, ExpressionFormat.General)
            {
                Serial = serial;
                Format = format;
            }

            public decimal Serial { get; }
            public ExpressionFormat Format { get; }

            internal override string ToString(Language language, ExpressionFormat info)
            {
                if (info == null)
                    info = Format;
                var date = FromDateSerial(Serial);
                if (date.HasValue)
                    return language.ToString(date.Value, info);
                return NA.ToString();
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
        private static ExcelValue MathOperation(ExcelValue a, ExcelValue b, Func<decimal, decimal, decimal> oper)
        {
            var a1 = a.AsDecimal();
            if (a1.HasValue)
            {
                var b1 = b.AsDecimal();
                if (b1.HasValue)
                {
                    var value = oper(a1.Value, b1.Value);
                    if (a is DateValue d1)
                        return new DateValue(value, d1.Language, d1.Format);
                    if (b is DateValue d2)
                        return new DateValue(value, d2.Language, d2.Format);
                    return new DecimalValue(value, a.Language);
                }
            }
            return VALUE;
        }

        public static ExcelValue operator +(ExcelValue a, ExcelValue b)
        {
            return MathOperation(a, b, (a, b) => a + b);
        }
        public static ExcelValue operator -(ExcelValue a, ExcelValue b)
        {
            return MathOperation(a, b, (a, b) => a - b);
        }
        public static ExcelValue operator *(ExcelValue a, ExcelValue b)
        {
            return MathOperation(a, b, (a, b) => a * b);
        }
        public static ExcelValue operator /(ExcelValue a, ExcelValue b)
        {
            var denominator = b.AsDecimal();
            if (denominator == 0M)
                return DIV0;
            return MathOperation(a, b, (a, b) => a / b);
        }
        public static ExcelValue operator -(ExcelValue a)
        {
            return a * MINUS_ONE;
        }
        public static ExcelValue operator ^(ExcelValue a, ExcelValue b)
        {
            return MathOperation(a, b, (a, b) => Convert.ToDecimal(Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b))));
        }
        public static ExcelValue operator &(ExcelValue a, ExcelValue b)
        {
            if (a is ErrorValue)
                return a;
            if (b is ErrorValue)
                return b;
            var value = $"{a.Text}{b.Text}";
            return new TextValue(value, a.Language);
        }
        #endregion

    }
}
