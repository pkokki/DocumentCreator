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
            ExpressionResult result = processor.Evaluate(0, expression, null);
            Assert.Null(result.Error);
            Assert.True(expected.Equals(result.Value, StringComparison.InvariantCulture), $"{result.Value} != {expected}");
        }
    }
}
