using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public abstract class BaseTest
    {
        protected readonly ITestOutputHelper output;
        protected readonly ExpressionEvaluator processor;
        protected readonly CultureInfo culture = CultureInfo.GetCultureInfo("el-GR");

        public BaseTest(ITestOutputHelper output)
        {
            this.output = output;
            processor = new ExpressionEvaluator(culture);
        }

        protected void AssertExpression(string expression, string expected)
        {
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(J25);"""";"\""");";";",");""", """;IFERROR(J25;"#N/A");""");")
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(B58);"""";"\""");";";",");""", """;IFERROR(B58;IF(ISNA(B58);"#N/A";"#VALUE!"));""");")
            var result = processor.Evaluate(expression);
            Assert.Null(result.Error);
            Assert.Equal(expected, result.Text);
        }

        protected void AssertExpression(string expression, Func<string, bool> assertion)
        {
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(J25);"""";"\""");";";",");""", """;IFERROR(J25;"#N/A");""");")
            // CONCATENATE("AssertExpression(""";SUBSTITUTE(SUBSTITUTE(FORMULATEXT(B58);"""";"\""");";";",");""", """;IFERROR(B58;IF(ISNA(B58);"#N/A";"#VALUE!"));""");")
            var result = processor.Evaluate(expression);
            Assert.Null(result.Error);
            Assert.True(assertion(result.Text), $"Value={result.Value}, Text={result.Text}");
        }
    }
}
