using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper output;
        protected readonly ExpressionEvaluator processor;

        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            processor = new ExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
        }

        protected void AssertExpression(string expression, string expected)
        {
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(J25);"""";"\""");";";",");""", """;IFERROR(J25;"#N/A");""");")
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(B58);"""";"\""");";";",");""", """;IFERROR(B58;IF(ISNA(B58);"#N/A";"#VALUE!"));""");")
            var result = processor.Evaluate(expression);
            Assert.Null(result.Error);
            Assert.Equal(expected, result.Text);
        }
    }
}
