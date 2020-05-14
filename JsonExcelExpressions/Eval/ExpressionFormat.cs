using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JsonExcelExpressions.Eval
{
    public class ExpressionFormat
    {
        public static readonly IDictionary<int, ValueFormat> standardFormats
            = new ConcurrentDictionary<int, ValueFormat>();
        public static readonly IDictionary<string, ValueFormat> customFormats
            = new ConcurrentDictionary<string, ValueFormat>();

        public abstract class ValueFormat
        {
            public ValueFormat(string format) 
            {
                if (format != null && !format.Contains("{0"))
                    format = "{0:" + format + "}";
                Format = format; 
            }
            public string Format { get; }
            public abstract bool NeedsDate { get; }
        }
        public class NumberValueFormat : ValueFormat
        {
            public NumberValueFormat(string format) : base(format) { }
            public override bool NeedsDate => false;
        }
        public class DateValueFormat : ValueFormat
        {
            public DateValueFormat(string format) : base(format) { }
            public override bool NeedsDate => true;
        }

        public static readonly ExpressionFormat General = new ExpressionFormat(0);
        public static readonly ExpressionFormat ShortDatePattern = new ExpressionFormat(14);
        public static readonly ExpressionFormat ShortTimePattern = new ExpressionFormat(18);

        public static ExpressionFormat CreateNumeric(int decimals) 
        {
            var customFormat = $"N{decimals}";
            return new ExpressionFormat(customFormats.TryGetAndAdd(customFormat, () => new NumberValueFormat(customFormat))); 
        }
        public static ExpressionFormat CreateFixedPoint(int decimals) 
        {
            var customFormat = $"F{decimals}";
            return new ExpressionFormat(customFormats.TryGetAndAdd(customFormat, () => new NumberValueFormat(customFormat)));
        }

        private readonly int? numFormatId;
        private readonly string numFormat;
        private readonly NumberFormatInfo numberFormatInfo;
        private readonly ValueFormat format;

        private ExpressionFormat(ValueFormat format)
        {
            this.format = format;
        }
        public ExpressionFormat(int? numFormatId)
        {
            this.numFormatId = numFormatId;
        }
        public ExpressionFormat(int? numFormatId, string numFormat, NumberFormatInfo numberFormatInfo)
        {
            this.numFormatId = numFormatId;
            this.numFormat = numFormat;
            this.numberFormatInfo = numberFormatInfo;
        }

        public ValueFormat GetFormat(ExpressionFormat defaultFormat)
        {
            var format = Resolve();
            if (format == null && defaultFormat != null)
                format = defaultFormat.Resolve();
            return format ?? TranslateExcelStandardFormat(0);
        }

        private ValueFormat Resolve()
        {
            ValueFormat format = null;
            if (this.format == null)
            {
                if (numFormatId.HasValue && numFormatId >= 0 && numFormatId < 164)
                    format = TranslateExcelStandardFormat(numFormatId.Value);
                else if (numFormat != null)
                    format = TranslateExcelFormat(numFormat);
            }
            else
            {
                format = this.format;
            }
            
            return format;
        }
        
        private ValueFormat TranslateExcelStandardFormat(int numFormatId)
        {
            switch (numFormatId)
            {
                case 0: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("G"));
                case 1: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("F0"));
                case 2: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("F2"));
                case 3: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("N0"));
                case 4: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("N2"));
                case 5:
                case 6: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("C0"));
                case 7:
                case 8: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("C2"));
                case 9: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("P0"));
                case 10: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("P2"));
                case 11: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("E2"));
                case 12: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,-9:F0}"));
                //case 13: return "# ??/??";
                case 14: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("d"));
                case 15: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("{0:d-MMM-yy}"));
                case 16: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("{0:d-MMM}"));
                case 17: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("{0:MMM-yy}"));
                case 18: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("t"));
                case 19: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("T"));
                case 20: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("H:mm"));
                case 21: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("H:mm:ss"));
                case 22: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("{0:d} {0:H:mm}"));

                case 37:
                case 38: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,-8:N0}"));
                case 39:
                case 40: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,-11:N2}"));
                case 41: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,7:N0}   "));
                case 42: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,9:C0} "));
                case 43: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,10:N2}   "));
                case 44: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,12:C2} "));

                case 45: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("mm:ss"));
                case 46: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("[h]:mm:ss"));
                case 47: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("mm:ss"));
                case 48: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0:#0.0E+0}"));

                case 49: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("G10"));
            }
            return null;
        }

        private ValueFormat TranslateExcelFormat(string format)
        {
            bool isDate = false;
            var sb = new StringBuilder();
            foreach (var ch1 in format)
            {
                var ch2 = ch1;
                var nf = numberFormatInfo;
                if (nf.NumberDecimalSeparator[0] == ch1)
                    ch2 = '.';
                else if (nf.NumberGroupSeparator[0] == ch1)
                    ch2 = ',';
                sb.Append(ch2);
                if (ch2 == 'd' || ch2 == 'M' || ch2 == 'y' || ch2 == 'H' || ch2 == 'h' || ch2 == 'm' || ch2 == 's')
                    isDate = true;
            }
            var customFormat = sb.ToString();
            if (isDate)
                return customFormats.TryGetAndAdd(customFormat, () => new DateValueFormat(customFormat));
            return customFormats.TryGetAndAdd(customFormat, () => new NumberValueFormat(customFormat));
        }
    }
}
