using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue ROWS(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array))
                return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ?  ExcelValue.ONE : ExcelValue.VALUE;
            return new ExcelValue.DecimalValue(array.Values.Count(), scope.OutLanguage);
        }

        public ExcelValue COLUMNS(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array)) 
                return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ?  ExcelValue.ONE : ExcelValue.VALUE;
            if (array.Values.FirstOrDefault() is ExcelValue.ArrayValue row)
                return new ExcelValue.DecimalValue(row.Values.Count(), scope.OutLanguage);
            return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ?  ExcelValue.ONE : ExcelValue.VALUE;
        }

        public ExcelValue INDEX(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array)) return ExcelValue.REF;
            if (args.NotNegInteger(1, int.MaxValue, out int rowNum)) return ExcelValue.REF;
            if (args.NotNegInteger(2, int.MaxValue, out int columnNum)) return ExcelValue.REF;
            if (rowNum == int.MaxValue && columnNum == int.MaxValue) return ExcelValue.REF;

            if (rowNum == int.MaxValue) rowNum = 1;
            if (columnNum == int.MaxValue) columnNum = 1;
            if (rowNum == 0)
            {
                if (columnNum == 0)
                    return array;
                return array.GetColumn(columnNum);
            }
            else 
            {
                var row = array.GetRow(rowNum);
                if (columnNum == 0)
                    return row;
                return row.ElementAt(columnNum);
            }
        }

        public ExcelValue LOOKUP(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.Count() < 2) return ExcelValue.NA;
            var lookupValue = args[0];
            if (!lookupValue.SingleValue) return ExcelValue.VALUE;
            if (args.NotArray(1, null, out ExcelValue.ArrayValue lookupVector)) return ExcelValue.REF;
            if (!lookupVector.IsVector) return ExcelValue.VALUE;
            if (args.NotArray(2, lookupVector, out ExcelValue.ArrayValue resultVector)) return ExcelValue.REF;

            var result = ExcelValue.NA;
            foreach (var pair in lookupVector.Values.Zip(resultVector.Values, (a, b) => new { A = a, B = b }))
            {
                result = pair.B;
                if (pair.A.CompareTo(lookupValue) >= 0)
                    break;
            }
            return result;
        }
    }
}
