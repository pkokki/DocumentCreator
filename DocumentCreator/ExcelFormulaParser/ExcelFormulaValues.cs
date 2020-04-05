using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExcelFormulaValues : List<ExcelFormulaValue>
    {
        public int IndexOf(ExcelFormulaTokenType type, params string[] values)
        {
            return FindIndex(p => p.HasToken && p.Token.Type == type && values.Contains(p.Token.Value));
        }
        public int IndexOf(ExcelFormulaTokenType type, string value)
        {
            return FindIndex(p => p.HasToken && p.Token.Type == type && p.Token.Value == value);
        }
        public int IndexOf(ExcelFormulaTokenType type, ExcelFormulaTokenSubtype subType)
        {
            return FindIndex(p => p.HasToken && p.Token.Type == type && p.Token.Subtype == subType);
        }

        public ExcelFormulaValue GetAndRemoveAt(int index)
        {
            var item = this[index];
            RemoveAt(index);
            return item;
        }
    }
}
