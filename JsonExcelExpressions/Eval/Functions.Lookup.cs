﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace JsonExcelExpressions.Eval
{
    internal partial class Functions
    {
        public ExcelValue ROWS(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array))
                return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ? ExcelValue.ONE : ExcelValue.VALUE;
            return new ExcelValue.DecimalValue(array.Values.Count(), scope.OutLanguage);
        }

        public ExcelValue COLUMNS(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array))
                return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ? ExcelValue.ONE : ExcelValue.VALUE;
            if (array.Values.FirstOrDefault() is ExcelValue.ArrayValue row)
                return new ExcelValue.DecimalValue(row.Values.Count(), scope.OutLanguage);
            return args.ElementAtOrDefault(0) is ExcelValue.DecimalValue ? ExcelValue.ONE : ExcelValue.VALUE;
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
                if (pair.A.CompareTo(lookupValue) > 0)
                    break;
                result = pair.B;
            }
            return result;
        }

        public ExcelValue HLOOKUP(List<ExcelValue> args, ExpressionScope scope)
        {
            // HLOOKUP(lookup_value, table_array, row_index_num, [range_lookup])
            if (args.Count() < 3) return ExcelValue.NA;
            var lookupValue = args[0];
            if (!lookupValue.SingleValue) return ExcelValue.VALUE;
            if (args.NotArray(1, null, out ExcelValue.ArrayValue tableArray)) return ExcelValue.REF;
            if (args.NotPosInteger(2, null, out int rowNum)) return ExcelValue.VALUE;
            if (args.NotBoolean(3, true, out bool approximateMatch)) return ExcelValue.VALUE;

            if (!(tableArray.GetRow(1) is ExcelValue.ArrayValue lookupVector)) return ExcelValue.REF;
            if (!(tableArray.GetRow(rowNum) is ExcelValue.ArrayValue resultVector)) return ExcelValue.REF;

            string pattern = null;
            var matchMode = approximateMatch ? -1 : (ExcelCriteria.IsRegex(args[0].Text, out pattern) ? 2 : 0);

            return XLOOKUP(args[0], lookupVector, resultVector, ExcelValue.NA, matchMode, 2, pattern);
        }

        public ExcelValue VLOOKUP(List<ExcelValue> args, ExpressionScope scope)
        {
            // VLOOKUP (lookup_value, table_array, col_index_num, [range_lookup])
            var argsCount = args.Count();
            if (argsCount < 3) return ExcelValue.NA;
            if (args.NotArray(1, null, out ExcelValue.ArrayValue tableArray)) return ExcelValue.REF;
            if (args.NotPosInteger(2, null, out int colNum)) return ExcelValue.VALUE;
            args.NotBoolean(3, true, out bool approxMatch);

            if (!(tableArray.GetColumn(1) is ExcelValue.ArrayValue lookupVector)) return ExcelValue.REF;
            if (!(tableArray.GetColumn(colNum) is ExcelValue.ArrayValue resultVector)) return ExcelValue.REF;

            string pattern = null;
            var matchMode = approxMatch ? -1 : (ExcelCriteria.IsRegex(args[0].Text, out pattern) ? 2 : 0);

            return XLOOKUP(args[0], lookupVector, resultVector, ExcelValue.NA, matchMode, 2, pattern);
        }

        public ExcelValue XLOOKUP(List<ExcelValue> args, ExpressionScope scope)
        {
            // XLOOKUP(lookup_value, lookup_array, return_array, [if_not_found], [match_mode], [search_mode])
            var argsCount = args.Count();
            if (argsCount < 3) return ExcelValue.NA;

            var lookupValue = args[0];
            if (!lookupValue.SingleValue) return ExcelValue.VALUE;
            if (args.NotArray(1, null, out ExcelValue.ArrayValue lookupArray)) return ExcelValue.REF;
            if (args.NotArray(2, null, out ExcelValue.ArrayValue returnArray)) return ExcelValue.REF;
            var ifNotFound = ExcelValue.NA;
            if (argsCount > 3 && args[3] != null) ifNotFound = args[3];
            if (args.NotInteger(4, 0, out int matchMode)) return ExcelValue.VALUE;
            if (args.NotInteger(5, 1, out int searchMode)) return ExcelValue.VALUE;
            if (searchMode < -2 || searchMode > 2 || searchMode == 0) return ExcelValue.VALUE;
            string pattern = null;
            if (matchMode == 2)
            {
                if (!ExcelCriteria.IsRegex(lookupValue.Text, out pattern))
                    return ExcelValue.VALUE;
            }

            return XLOOKUP(lookupValue, lookupArray, returnArray, ifNotFound, matchMode, searchMode, pattern);
        }

        private ExcelValue XLOOKUP(ExcelValue lookupValue, ExcelValue.ArrayValue lookupArray, ExcelValue.ArrayValue returnArray,
            ExcelValue ifNotFound, int matchMode, int searchMode, string pattern)
        {
            Func<ExcelValue, int> comparer;
            switch (matchMode)
            {
                case 0: // Exact match. If none found, return #N/A. This is the default.
                case -1: // Exact match. If none found, return the next smaller item.
                case 1: // Exact match. If none found, return the next larger item.
                    comparer = v => v.CompareTo(lookupValue);
                    break;
                case 2: // A wildcard match where *, ?, and ~ have special meaning.
                    comparer = v => Regex.IsMatch(v.Text, pattern, RegexOptions.IgnoreCase) ? 0 : -1;
                    break;
                default:
                    return ExcelValue.VALUE;
            }

            IEnumerable<ExcelValue> lookupValues, returnValues;
            if (searchMode > 0)
            {
                lookupValues = lookupArray.Values;
                returnValues = returnArray.Values;
            }
            else
            {
                lookupValues = lookupArray.Values.Reverse();
                returnValues = returnArray.Values.Reverse();
            }

            var expectSorted = Math.Abs(searchMode) == 2;
            ExcelValue resultA = null;
            ExcelValue resultB = null;
            foreach (var pair in lookupValues.Zip(returnValues, (a, b) => new { A = a, B = b }))
            {
                var comparison = comparer(pair.A);
                if (comparison == 0)
                    resultB = pair.B;
                else if (comparison < 0 && matchMode == -1)
                {
                    if (resultB == null || pair.A.CompareTo(resultA) > 0)
                    {
                        resultA = pair.A;
                        resultB = pair.B;
                    }
                }
                else if (comparison > 0 && matchMode == 1)
                {
                    if (resultB == null || pair.A.CompareTo(resultA) < 0)
                    {
                        resultA = pair.A;
                        resultB = pair.B;
                    }
                }
                if (comparison == 0 || (expectSorted && comparison > 0)) break;
            }
            return resultB ?? ifNotFound;
        }

        public ExcelValue MATCH(List<ExcelValue> args, ExpressionScope scope)
        {
            // MATCH(lookup_value, lookup_array, [match_type])
            var argsCount = args.Count();
            if (argsCount < 2) return ExcelValue.NA;
            var lookupValue = args[0];
            if (!lookupValue.SingleValue) return ExcelValue.VALUE;
            if (args.NotArray(1, null, out IEnumerable<ExcelValue> lookupArray)) return ExcelValue.REF;
            if (args.NotInteger(2, 1, out int matchType)) return ExcelValue.VALUE;
            if (matchType < -1 || matchType > 1) return ExcelValue.VALUE;

            var index = 1;
            var count = lookupArray.Count();
            var step = 1;
            if (matchType == -1)
            {
                lookupArray = lookupArray.Reverse();
                index = count;
                step = -1;
            }
            foreach (var item in lookupArray)
            {
                var comparison = item.CompareTo(lookupValue);
                if (comparison == 0)
                {
                    return new ExcelValue.DecimalValue(index, scope.OutLanguage);
                }
                else if (comparison == 1)
                {
                    if (matchType == 0)
                        return ExcelValue.NA;
                    break;
                }
                index += step;
            }
            if (matchType == 0)
                return ExcelValue.NA;
            if (matchType == 1 && index > 1)
                --index;
            if (index >= 1 && index <= count)
                return new ExcelValue.DecimalValue(index, scope.OutLanguage);
            return ExcelValue.NA;
        }

        public ExcelValue CHOOSE(List<ExcelValue> args, ExpressionScope scope)
        {
            // CHOOSE(index_num, value1, [value2], ...)
            if (args.NotPosInteger(0, null, out int index)) return ExcelValue.VALUE;


            if (index < args.Count)
                return args[index];
            return ExcelValue.NA;
        }
        
        public ExcelValue HYPERLINK(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string linkUrl)) return ExcelValue.VALUE;
            var linkText = args.Count > 1 ? args[1].Text : null;
            if (!Uri.IsWellFormedUriString(linkUrl, UriKind.Absolute))
                return ExcelValue.VALUE;

            return new ExcelValue.HyperlinkValue(linkUrl, linkText, scope.OutLanguage);
        }

        public ExcelValue UNIQUE(List<ExcelValue> args, ExpressionScope scope)
        {
            // UNIQUE(array,[by_col],[exactly_once])
            if (args.NotArray(0, null, out ExcelValue.ArrayValue array)) return ExcelValue.VALUE;
            if (args.NotBoolean(1, false, out bool byColumn)) return ExcelValue.VALUE;
            if (args.NotBoolean(2, false, out bool exactlyOnce)) return ExcelValue.VALUE;

            var unique = array.Values
                .GroupBy(o => o.InnerValue)
                .Where(o => exactlyOnce ? o.Count() == 1 : true)
                .Select(o => o.First());
            return new ExcelValue.ArrayValue(unique, scope.OutLanguage);
        }

        public ExcelValue SORTBY(List<ExcelValue> args, ExpressionScope scope)
        {
            // SORTBY(array, by_array1, [sort_order1], [by_array2, sort_order2],…) 
            if (args.NotArray(0, null, out ExcelValue.ArrayValue arrayToSort)) return ExcelValue.VALUE;
            var argsCount = args.Count;
            if (argsCount < 2) return ExcelValue.NA;

            var matrix = arrayToSort.Values.Select(v => new List<ExcelValue> { v }).ToList();
            var sorters = argsCount / 2;
            var index = 1;
            var asc = new List<int> { 0 };
            while (index < argsCount)
            {
                if (args.NotArray(1, null, out ExcelValue.ArrayValue sortBy)) return ExcelValue.VALUE;
                ++index;
                if (args.NotInteger(2, 1, out int sortOrder)) return ExcelValue.VALUE;
                if (sortOrder != 1 && sortOrder != -1) return ExcelValue.VALUE;
                asc.Add(sortOrder);
                ++index;
                
                var arraySortBy = sortBy.Values.ToArray();
                for (var i = 0; i < matrix.Count; i++)
                    matrix[i].Add(arraySortBy[i]);
            }
            matrix.Sort((a1, a2) => 
            {
                var result = -1;
                var column = 1;
                do
                {
                    result = asc[column] * a1[column].CompareTo(a2[column]);
                    ++column;
                } while (result != 0 && column <= sorters);
                return result;
            });
            return new ExcelValue.ArrayValue(matrix.Select(r => r[0]), scope.OutLanguage);
        }

        public ExcelValue INDIRECT(List<ExcelValue> args, ExpressionScope scope)
        {
            if (args.NotText(0, null, scope.OutLanguage, out string refText)) return ExcelValue.VALUE;

            var newArgs = new List<ExcelValue> 
            {
                new ExcelValue.TextValue("N3", scope.OutLanguage), 
                new ExcelValue.TextValue(refText, scope.OutLanguage)
            };
            return MAPVALUE(newArgs, scope);
        }
    }
}
