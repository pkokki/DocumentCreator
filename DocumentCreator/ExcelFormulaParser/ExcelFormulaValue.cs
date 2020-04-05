using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExcelFormulaValue
    {
        private readonly ExcelFormulaToken token;
        private readonly object value;

        public ExcelFormulaValue(ExcelFormulaToken token) 
        {
            this.token = token;
        }
        public ExcelFormulaValue(ExcelFormulaToken token, object value)
        {
            if (token != null) throw new InvalidOperationException();
            this.value = value;
        }

        public bool HasValue { get { return value != null; } }
        public object Value
        {
            get
            {
                if (token != null && token.Type != ExcelFormulaTokenType.Operand) throw new InvalidOperationException();
                return value ?? token?.Value;
            }
        }
        public bool HasToken { get { return token != null; } }
        public ExcelFormulaToken Token
        {
            get
            {
                if (token == null) throw new InvalidOperationException();
                return token;
            }
        }
    }
}
