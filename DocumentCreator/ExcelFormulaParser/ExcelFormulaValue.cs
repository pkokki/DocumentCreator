using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExcelFormulaValue
    {
        private readonly ExcelFormulaToken token;
        private readonly CultureInfo culture;
        private ExcelValue value;

        public ExcelFormulaValue(ExcelFormulaToken token, CultureInfo culture) 
        {
            this.token = token;
            this.culture = culture;
        }
        public ExcelFormulaValue(ExcelValue value)
        {
            this.value = value;
        }

        public bool HasValue { get { return value != null; } }
        public ExcelValue Value
        {
            get
            {
                if (token != null && token.Type != ExcelFormulaTokenType.Operand) throw new InvalidOperationException();
                if (value == null && token != null)
                    value = ExcelValue.Create(token, culture);
                return value;
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
