using JsonExcelExpressions.Lang;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormulaParser
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper output;
        protected readonly MappingExpressionEvaluator processor;
        protected readonly Language language;

        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            language = Language.ElGr;
            processor = new MappingExpressionEvaluator(Language.Invariant, language);
        }

        protected void AssertExpression(string expression, string expected)
        {
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(J25);"""";"\""");";";",");""", """;IFERROR(J25;"#N/A");""");")
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(B58);"""";"\""");";";",");""", """;IFERROR(B58;IF(ISNA(B58);"#N/A";"#VALUE!"));""");")
            var result = processor.Evaluate("F01", "F1", expression, null);
            Assert.Null(result.Error);
            Assert.Equal(expected, result.Text);
        }
    }
}
