using DocumentCreator.ExcelFormula;
using DocumentCreator.ExcelFormulaParser.Languages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.ExcelFormula
{
    public class Dates : BaseTest
    {
        public Dates(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void DATE()
        {
            AssertExpression("=DATE(2020,4,18)", "18/4/2020");
            AssertExpression("=DATE(2020,4,18)+60", "17/6/2020");
            AssertExpression("=DATE(2020,4,18)-60", "18/2/2020");
            AssertExpression("=DATE(2020,4,18)-0.42", "17/4/2020");
            AssertExpression("=DATE(2020,4,18)+0.42", "18/4/2020");
            AssertExpression("=DATE(2020,4,18)*0.42", "10/7/1950");

            var enProcessor = new ExpressionEvaluator(Language.Invariant, new Language(CultureInfo.GetCultureInfo("en-US")));
            var result = enProcessor.Evaluate("F01", "F1", "=DATE(2020,4,18)", null);
            Assert.Null(result.Error);
            Assert.Equal("4/18/2020", result.Text);
        }

        [Fact]
        public void TIME()
        {
            AssertExpression("=TIME(11,6,43)", "11:06 πμ");
            AssertExpression("=TIME(23,6,43)", "11:06 μμ");
            AssertExpression("=TIME(11,6,43)+0.3", "6:18 μμ");
            AssertExpression("=TIME(11,6,43)-0.3", "3:54 πμ");
            AssertExpression("=TIME(11,6,43)*0.3", "3:20 πμ");
            AssertExpression("=TIME(11,6,43)*4.32", "12:00 πμ");
            AssertExpression("=TIME(11,6,43)+4.32", "6:47 μμ");

            var enProcessor = new ExpressionEvaluator(Language.Invariant, new Language(CultureInfo.GetCultureInfo("en-US")));
            var result = enProcessor.Evaluate("F01", "F1", "=TIME(11,6,43)", null);
            Assert.Null(result.Error);
            Assert.Equal("11:06 AM", result.Text);
        }

        [Fact]
        public void NOW()
        {
            AssertExpression("=NOW()", DateTime.Now.ToShortDateString());
            AssertExpression("=NOW()+123", (DateTime.Now.AddDays(123)).ToShortDateString());
        }
    }
}
