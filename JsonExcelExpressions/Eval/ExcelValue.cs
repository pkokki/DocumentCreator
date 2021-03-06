﻿using JsonExcelExpressions.Lang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    public abstract class ExcelValue : IComparable<ExcelValue>
    {
        #region Singletons

        public static readonly ExcelValue NA = new ErrorValue("#N/A");
        public static readonly ExcelValue VALUE = new ErrorValue("#VALUE!");
        public static readonly ExcelValue DIV0 = new ErrorValue("#DIV/0!");
        public static readonly ExcelValue REF = new ErrorValue("#REF!");
        public static readonly ExcelValue TRUE = BooleanValue._TRUE;
        public static readonly ExcelValue FALSE = BooleanValue._FALSE;
        public static readonly ExcelValue NULL = new NullValue();
        public static readonly ExcelValue ZERO = new DecimalValue(0, Language.Invariant);
        public static readonly ExcelValue ONE = new DecimalValue(1, Language.Invariant);
        public static readonly ExcelValue MINUS_ONE = new DecimalValue(-1, Language.Invariant);
        public static readonly ExcelValue HUNDRED = new DecimalValue(100, Language.Invariant);

        #endregion

        #region Static methods
        public static double? ToDateSerial(DateTime date)
        {
            return ToDateSerial(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
        public static double? ToDateSerial(int year, int month, int day, int hours = 0, int minutes = 0, int seconds = 0)
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
                return serial;
            }
            else
            {
                var serial = Math.Round((hours * 60 * 60 + minutes * 60 + seconds) / 86400.0, 10);
                if (serial < 0)
                    return null;
                return serial;
            }
        }

        public static DateTime? FromDateSerial(double serial)
        {
            if (serial > 0 && serial < 1)
            {
                var seconds = Math.Round(serial * 86400.0);
                return DateValue.BASE.AddSeconds(seconds);
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
            if (token == null)
                return NA;
            switch (token.Type)
            {
                case JTokenType.Object: return new JsonObjectValue((JObject)token, language);
                case JTokenType.Array: return new ArrayValue((JArray)token, language);
                case JTokenType.Boolean: return BooleanValue.Create((bool)token);
                case JTokenType.Integer:
                case JTokenType.Float:
                    return new DecimalValue((double)token, language);
                default: return new TextValue(token.ToString(), language);
            }
        }

        internal static ExcelValue Create(ExcelFormulaToken token, ExpressionScope scope)
        {
            switch(token.Subtype)
            {
                case ExcelFormulaTokenSubtype.Text: return new TextValue(token.Value, scope.OutLanguage);
                case ExcelFormulaTokenSubtype.Number: return new DecimalValue(double.Parse(token.Value, CultureInfo.InvariantCulture), scope.OutLanguage);
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
                    return BooleanValue.Create(((double)v1.InnerValue).IsEqual((double)v2.InnerValue));
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
                    return BooleanValue.Create(((double)v1.InnerValue).CompareWith((double)v2.InnerValue) > 0);
                return BooleanValue.Create(string.Compare(v1.InnerValue.ToString(), v2.InnerValue.ToString(), ignoreCase) > 0);

            }
            if (oper == ">=")
            {
                if (v1 is BooleanValue) return BooleanValue.Create(!(v1 == FALSE && v2 == TRUE));
                if ((v1 is TextValue && v2 is TextValue)) // *************** || (v1 is JsonTextValue && v2 is JsonTextValue))
                    return BooleanValue.Create(string.Compare((string)v1.InnerValue, (string)v2.InnerValue, ignoreCase) >= 0);
                if (v1 is DecimalValue && v2 is DecimalValue)
                    return BooleanValue.Create(((double)v1.InnerValue).CompareWith((double)v2.InnerValue) >= 0);
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
        public virtual bool SingleValue => true;

        #endregion

        #region Methods 

        protected internal abstract bool? AsBoolean();
        protected internal abstract double? AsDecimal();
        internal abstract string ToString(Language language, ExpressionFormat info);

        public override string ToString()
        {
            return ToString(Language, null);
        }
        public virtual ExcelValue ElementAt(int index)
        {
            return REF;
        }

        public abstract int CompareTo(ExcelValue other);

        #endregion

        #region Private classes
        internal class ErrorValue : ExcelValue
        {
            internal ErrorValue(string text) : base(null, text, Language.Invariant)
            {
            }

            public override bool SingleValue => false;

            internal override string ToString(Language language, ExpressionFormat info) { return Text; }

            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal() { return null; }

            public override int CompareTo(ExcelValue other) => this == other || other == NULL ? 0 : 1;
        }

        internal class NullValue : ExcelValue
        {
            public NullValue() : base(null, null, Language.Invariant)
            {
            }
            internal override string ToString(Language language, ExpressionFormat info) { return string.Empty; }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal() { return null; }
            public override int CompareTo(ExcelValue other) => this == other ? 0 : 1;
        }

        internal class TextValue : ExcelValue
        {
            public TextValue(string text, Language language) : base(text, text, language)
            {
            }
            public TextValue(object value, string text, Language language) : base(value, text, language)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal()
            {
                if (Language.TryParseDecimal(Text, out double v))
                    return v;
                if (Language.TryParseDateTime(Text, out DateTime d))
                    return DateValue.ToDateSerial(d);
                return null;
            }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => other is DecimalValue ? 1 : (other is TextValue ? Text.CompareTo(other.Text): -1);
        }

        internal class HyperlinkValue : TextValue
        {
            public HyperlinkValue(string url, string title, Language language) 
                : base(JObject.Parse($"{{ url: \"{url}\", text: \"{title ?? url}\" }}"), url, language)
            {
                Title = title ?? url;
            }

            public string Title { get; }
        }

        internal class JsonObjectValue : ExcelValue
        {
            public JsonObjectValue(JObject token, Language language) : base(token, "{}", language)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => -1;
        }

        internal class ArrayValue : ExcelValue
        {
            private readonly bool? asBoolean;
            private readonly double? asDecimal;
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

            public ExcelValue GetRow(int rowNum)
            {
                if (rowNum < 1 || rowNum > values.Count())
                    return REF;
                return values.ElementAt(rowNum - 1);
            }
            public ExcelValue GetColumn(int colNum)
            {
                if (colNum < 1)
                    return REF;
                var column = values.Select(v => v.ElementAt(colNum));
                if (column.Any(o => o == REF)) return REF;
                return new ArrayValue(column, Language);
            }
            public override ExcelValue ElementAt(int num)
            {
                return values.ElementAtOrDefault(num - 1) ?? REF;
            }
            public override bool SingleValue => false;

            public bool IsVector
            {
                get { return values.All(o => o.SingleValue); }
            }

            protected internal override bool? AsBoolean() { return asBoolean; }
            protected internal override double? AsDecimal() { return asDecimal; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => -1;
        }

        internal class SourceReferenceValue : ExcelValue
        {
            public SourceReferenceValue(string sourceName) : base(null, sourceName, Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => -1;
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
            protected internal override double? AsDecimal() { return (bool)InnerValue ? 1 : 0; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => other is ErrorValue ? -1 : (other is BooleanValue ? ((bool)InnerValue).CompareTo((bool)other.InnerValue) : 1);
        }

        internal class RangeValue : ExcelValue
        {
            public RangeValue(string value) : base(value, value, Language.Invariant)
            {
            }
            protected internal override bool? AsBoolean() { return null; }
            protected internal override double? AsDecimal() { return null; }
            internal override string ToString(Language language, ExpressionFormat info) { return Text; }
            public override int CompareTo(ExcelValue other) => -1;
        }

        internal class DateValue : DecimalValue
        {
            internal static readonly DateTime BASE = new DateTime(1900, 1, 1);
            
            public DateValue(double serial, Language language, ExpressionFormat format)
                : base(serial, language, ExpressionFormat.General)
            {
                Serial = serial;
                Format = format;
            }

            public double Serial { get; }
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
            public DecimalValue(double value, Language language, ExpressionFormat format = null)
                : base(value, language.ToString(value, format), language)
            {
            }
            protected internal override bool? AsBoolean() { return ((double)InnerValue) != 0; }
            protected internal override double? AsDecimal() { return (double)InnerValue; }
            internal override string ToString(Language language, ExpressionFormat info) 
            {
                if (info != null)
                    return language.ToString((double)InnerValue, info);
                return Text;
            }
            public override int CompareTo(ExcelValue other) => other is DecimalValue ? ((double)InnerValue).CompareTo((double)other.InnerValue) : -1;
        }

        #endregion

        #region Operators
        private static ExcelValue MathOperation(ExcelValue a, ExcelValue b, Func<double, double, double> oper)
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
            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x + y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x + b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a + x), b.Language);
            }
            return MathOperation(a, b, (a, b) => a + b);
        }
        public static ExcelValue operator -(ExcelValue a, ExcelValue b)
        {
            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x - y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x - b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a - x), b.Language);
            }
            return MathOperation(a, b, (a, b) => a - b);
        }
        public static ExcelValue operator *(ExcelValue a, ExcelValue b)
        {
            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x * y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x * b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a * x), b.Language);
            }
            return MathOperation(a, b, (a, b) => a * b);
        }
        public static ExcelValue operator /(ExcelValue a, ExcelValue b)
        {
            var denominator = b.AsDecimal();
            if (denominator == 0.0)
                return DIV0;
            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x / y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x / b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a / x), b.Language);
            }
            return MathOperation(a, b, (a, b) => a / b);
        }
        public static ExcelValue operator -(ExcelValue a)
        {
            if (a is ArrayValue arr1)
                return new ArrayValue(arr1.Values.Select(x => -x), a.Language);
            return a * MINUS_ONE;
        }
        public static ExcelValue operator ^(ExcelValue a, ExcelValue b)
        {
            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x ^ y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x ^ b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a ^ x), b.Language);
            }
            return MathOperation(a, b, (a, b) => Math.Pow(a, b));
        }
        public static ExcelValue operator &(ExcelValue a, ExcelValue b)
        {
            if (a is ErrorValue)
                return a;
            if (b is ErrorValue)
                return b;

            if (a is ArrayValue arr1)
            {
                if (b is ArrayValue arr2)
                    return new ArrayValue(arr1.Values.Zip(arr2.Values, (x, y) => x & y), a.Language);
                else
                    return new ArrayValue(arr1.Values.Select(x => x & b), a.Language);
            }
            else if (b is ArrayValue arr2)
            {
                return new ArrayValue(arr2.Values.Select(x => a & x), b.Language);
            }
            var value = $"{a.Text}{b.Text}";
            return new TextValue(value, a.Language);
        }

        #endregion

    }
}
