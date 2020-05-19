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

        public static IEnumerable<double> FlattenNumbers(this IEnumerable<ExcelValue> args, bool ignoreErrorValues)
        {
            var numbers = new List<double>();
            foreach (var arg in args)
            {
                if (arg is ExcelValue.ArrayValue)
                {
                    var items = (IEnumerable<ExcelValue>)arg.InnerValue;
                    var childNumbers = items.FlattenNumbers(true); // excel ignores child error values
                    if (childNumbers != null)
                        numbers.AddRange(childNumbers);
                    //else if (!ignoreErrorValues)
                    //    return null;
                }
                else
                {
                    var number = arg.AsDecimal();
                    if (number.HasValue)
                        numbers.Add(number.Value);
                    else if (!ignoreErrorValues)
                        return null;
                }
            }
            return numbers;
        }

        public static bool NotDecimal(this List<ExcelValue> args, int index, double? defaultValue, out double value)
        {
            double? result = null;
            if (args.Count > index)
            {
                var v = args[index].AsDecimal();
                if (v.HasValue)
                    result = v.Value;
            }
            if (result == null && defaultValue.HasValue)
                result = defaultValue.Value;
            value = result ?? 0.0;
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
                value = args[index].ToString(language, null);
            if (value == null)
                value = defaultValue;
            return value == null;
        }

        public static bool NotArray(this IEnumerable<ExcelValue> args, int index, ExcelValue.ArrayValue defaultValue, out ExcelValue.ArrayValue value)
        {
            value = null;
            if (args.Count() > index && args.ElementAt(index) is ExcelValue.ArrayValue arr)
                value = arr;
            if (value == null)
                value = defaultValue;
            return value == null;
        }
        public static bool NotArray(this List<ExcelValue> args, int index, IEnumerable<ExcelValue> defaultValue, out IEnumerable<ExcelValue> value)
        {
            value = null;
            if (args.Count > index && args[index] is ExcelValue.ArrayValue arr)
                value = arr.Values;
            if (value == null)
                value = defaultValue;
            return value == null;
        }

        private static readonly object theLock = new object();
        public static TValue TryGetAndAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> factory)
        {
            lock (theLock)
            {
                if (dict.TryGetValue(key, out TValue value))
                    return value;
                value = factory();
                dict.Add(key, value);
                return value;
            }
        }

        public static bool IsEqual(this double v1, double? v2)
        {
            if (v2.HasValue)
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.double.equals?view=netcore-3.1
                // Define the tolerance for variation in their values
                double difference = Math.Abs(v1 * .00001);
                // Compare the values
                return Math.Abs(v1 - v2.Value) <= difference;
            }
            return false;
        }
        public static int CompareWith(this double v1, double v2)
        {
            if (v1.IsEqual(v2))
                return 0;
            return v1.CompareTo(v2);
        }
    }
}
