using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace JsonExcelExpressions.Eval
{
    public class ExpressionFormat
    {
        public static readonly Dictionary<int, ValueFormat> standardFormats
            = new Dictionary<int, ValueFormat>();
        public static readonly Dictionary<string, ValueFormat> customFormats
            = new Dictionary<string, ValueFormat>();

        public abstract class ValueFormat
        {
            public ValueFormat(string format) 
            {
                if (format != null && !format.StartsWith("{"))
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
            return format ?? defaultFormat.GetFormat(null);
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
                case 15: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("d-MMM-yy"));
                case 16: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("M"));
                case 17: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("Y"));
                case 18: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("t"));
                case 19: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("T"));
                case 20: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("H:mm"));
                case 21: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("H:mm:ss"));
                case 22: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("d H:mm"));
                case 30: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("m/d/yy"));

                case 37:
                case 38: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,-8:N0}"));
                case 39:
                case 40: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("{0,-11:N2}"));

                case 45: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("mm:ss"));
                case 46: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("[h]:mm:ss"));
                case 47: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("mmss.0"));
                case 48: return standardFormats.TryGetAndAdd(numFormatId, () => new DateValueFormat("##0.0E+0"));

                case 49: return standardFormats.TryGetAndAdd(numFormatId, () => new NumberValueFormat("G10"));
            }
            return null;
        }

        private ValueFormat TranslateExcelFormat(string format)
        {
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
            }
            var customFormat = sb.ToString();
            // TODO: need to separate DateValueFormat
            return customFormats.TryGetAndAdd(customFormat, () => new NumberValueFormat(customFormat));
        }
    }
}
