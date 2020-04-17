using DocumentCreator.ExcelFormulaParser.Languages;
using DocumentCreator.Model;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormula
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper output;
        protected readonly ExpressionEvaluator processor;
        protected readonly Language language;

        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            language = Language.ElGr;
            processor = new ExpressionEvaluator(Language.Invariant, language);
        }

        protected void AssertExpression(string expression, string expected)
        {
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(J25);"""";"\""");";";",");""", """;IFERROR(J25;"#N/A");""");")
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(B58);"""";"\""");";";",");""", """;IFERROR(B58;IF(ISNA(B58);"#N/A";"#VALUE!"));""");")
            var result = processor.Evaluate("F01", expression, null);
            Assert.Null(result.Error);
            Assert.Equal(expected, result.Text);
        }
    }
}
