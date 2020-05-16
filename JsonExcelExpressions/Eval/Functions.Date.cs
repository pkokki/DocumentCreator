using System;
using System.Collections.Generic;
using System.Text;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue NOW(List<ExcelValue> args, ExpressionScope scope)
        {
            var now = DateTime.Now;
            return ExcelValue.CreateDateValue(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, scope.OutLanguage, ExpressionFormat.ShortDatePattern);
        }
        public ExcelValue TODAY(List<ExcelValue> args, ExpressionScope scope)
        {
            var today = DateTime.Today;
            return ExcelValue.CreateDateValue(today.Year, today.Month, today.Day, 0, 0, 0, scope.OutLanguage, ExpressionFormat.ShortDatePattern);
        }
        public ExcelValue DATE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotInteger(0, null, out int year)) return ExcelValue.NA;
            if (args.NotInteger(1, null, out int month)) return ExcelValue.NA;
            if (args.NotInteger(2, null, out int day)) return ExcelValue.NA;
            return ExcelValue.CreateDateValue(year, month, day, scope.OutLanguage, ExpressionFormat.ShortDatePattern);
        }
        public ExcelValue TIME(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotInteger(0, null, out int hours)) return ExcelValue.NA;
            if (args.NotInteger(1, null, out int minutes)) return ExcelValue.NA;
            if (args.NotInteger(2, null, out int seconds)) return ExcelValue.NA;
            return ExcelValue.CreateDateValue(0, 0, 0, hours, minutes, seconds, scope.OutLanguage, ExpressionFormat.ShortTimePattern);
        }

        public ExcelValue DAY(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(0, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Day, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }
        public ExcelValue MONTH(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(1, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Month, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }
        public ExcelValue YEAR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(1900, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Year, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }

        public ExcelValue HOUR(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(0, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Hour, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }

        public ExcelValue MINUTE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(0, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Minute, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }

        public ExcelValue SECOND(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.NA;
            if (serial == 0)
                return new ExcelValue.DecimalValue(0, scope.OutLanguage, ExpressionFormat.General);
            var date = ExcelValue.FromDateSerial(serial);
            if (date.HasValue)
                return new ExcelValue.DecimalValue(date.Value.Second, scope.OutLanguage, ExpressionFormat.General);
            return ExcelValue.VALUE;
        }

        public ExcelValue DATEDIF(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial1)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out decimal serial2)) return ExcelValue.VALUE;

            if (serial1 < 0 || serial2 < 0 || serial1 > serial2) return ExcelValue.VALUE;

            var dv1 = ExcelValue.FromDateSerial(serial1);
            if (!dv1.HasValue) return ExcelValue.NA;
            var dv2 = ExcelValue.FromDateSerial(serial2);
            if (!dv2.HasValue) return ExcelValue.NA;

            if (args.NotText(2, null, scope.OutLanguage, out string unit)) return ExcelValue.NA;

            var d1 = dv1.Value;
            var d2 = dv2.Value;
            double result;
            switch (unit.ToUpperInvariant())
            {
                case "Y": 
                    result = d2.Year - d1.Year - (d2.Month >= d1.Month && d2.Day >= d1.Day ? 0 : 1); break;
                case "M": result = 12 * (d2.Year - d1.Year) + (d2.Month - d1.Month); break;
                case "D": result = Math.Round((d2 - d1).TotalDays); break;
                case "MD": result = d2.Day - d1.Day; break;
                case "YM": 
                    result = d2.Month - d1.Month;
                    if (result < 0)
                        result += 12;
                    break;
                case "YD":
                    var tmpD1 = new DateTime(d1.Year + (d2.Month >= d1.Month && d2.Day >= d1.Day ? 1 : 0), d1.Month, d1.Day);
                    result = Math.Round((d2 - tmpD1).TotalDays);
                    break;
                default: return ExcelValue.VALUE;
            }
            return new ExcelValue.DecimalValue((decimal)result, scope.OutLanguage);
        }

        public ExcelValue DAYS(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial1)) return ExcelValue.VALUE;
            if (args.NotDecimal(1, null, out decimal serial2)) return ExcelValue.VALUE;

            if (serial1 < 0 || serial2 < 0) return ExcelValue.VALUE;

            var days = Math.Truncate(serial1 - serial2);
            return new ExcelValue.DecimalValue(days, scope.OutLanguage, ExpressionFormat.General);
        }

        public ExcelValue DATEVALUE(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotDecimal(0, null, out decimal serial)) return ExcelValue.VALUE;

            return new ExcelValue.DecimalValue(serial, scope.OutLanguage, ExpressionFormat.General);

        }
    }
}
