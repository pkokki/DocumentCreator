using DocumentCreator.ExcelFormulaParser.Languages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.ExcelFormulaParser
{
    public class ExpressionContext
    {
        public ExpressionContext(Language inputLang, Language outputLang)
        {
            InputLang = inputLang;
            OutputLang = outputLang;
        }

        public Language InputLang { get; }
        public Language OutputLang { get; }
    }
}
