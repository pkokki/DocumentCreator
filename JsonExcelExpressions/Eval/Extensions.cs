using JsonExcelExpressions.Lang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    internal static class Extensions
    {
        public static bool ContainErrorValues(this List<ExcelValue> args)
        {
            return args.Any(a => a is ExcelValue.ErrorValue);
        }

        public static bool NotDecimal(this List<ExcelValue> args, int index, decimal? defaultValue, out decimal value)
        {
            decimal? result = null;
            if (args.Count > index)
            {
                var v = args[index].AsDecimal();
                if (v.HasValue)
                    result = v.Value;
            }
            if (result == null && defaultValue.HasValue)
                result = defaultValue.Value;
            value = result ?? 0M;
            return !result.HasValue;
        }
        public static bool NotPosInteger(this List<ExcelValue> args, int index, int? defaultValue, out int value)
        {
            if (!NotInteger(args, index, defaultValue, out value) && value > 0)
                return false;
            value = 0;
            return true;
        }
        public static bool NotNegInteger(this List<ExcelValue> args, int index, int? defaultValue, out int value)
        {
            if (!NotInteger(args, index, defaultValue, out value) && value >= 0)
                return false;
            value = 0;
            return true;
        }
        public static bool NotInteger(this List<ExcelValue> args, int index, int? defaultValue, out int value)
        {
            int? result = null;
            if (args.Count > index)
            {
                var v = args[index].AsDecimal();
                if (v.HasValue)
                    result = (int)Math.Truncate(v.Value);
            }
            if (result == null && defaultValue.HasValue)
                result = defaultValue.Value;
            value = result ?? 0;
            return !result.HasValue;
        }
        public static bool NotBoolean(this List<ExcelValue> args, int index, bool? defaultValue, out bool value)
        {
            bool? result = null;
            if (args.Count > index)
            {
                var v = args[index].AsBoolean();
                if (v.HasValue)
                    result = v.Value;
            }
            if (result == null && defaultValue.HasValue)
                result = defaultValue.Value;
            value = result ?? false;
            return !result.HasValue;
        }
        public static bool NotText(this List<ExcelValue> args, int index, string defaultValue, Language language, out string value)
        {
            value = null;
            if (args.Count > index)
                value = args[index].ToString(language);
            if (value == null)
                value = defaultValue;
            return value == null;
        }
    }
}
