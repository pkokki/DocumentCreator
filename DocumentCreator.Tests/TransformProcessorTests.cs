using DocumentCreator.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator
{
    public class TransformProcessorTests
    {
        private readonly ITestOutputHelper output;
        private readonly TransformProcessor processor;
        private readonly CultureInfo culture;

        public TransformProcessorTests(ITestOutputHelper output)
        {
            this.output = output;
            culture = CultureInfo.GetCultureInfo("el-GR");
            processor = new TransformProcessor(CultureInfo.InvariantCulture, culture);
        }

        [Fact]
        public void CanTestTransformations()
        {
            var testRequest = JObject.Parse(File.ReadAllText("./Resources/example01.json"));
            var response = processor.Test(testRequest);
            Assert.NotNull(response);
            Assert.NotEmpty(response.Results);
            foreach (var result in response.Results)
                output.WriteLine(result.ToString()); 
            Assert.True(response.Results.TrueForAll(r => r.Error == null));
        }

        [Fact]
        public void CanEvaluateMathOperations()
        {
            AssertExpression("=2*6>7+4", "TRUE");
            AssertExpression("=2*(3+4)", "14");
            AssertExpression("=1", "1");
            AssertExpression("=-2", "-2");
            AssertExpression("=1.1", "1,1");
            AssertExpression("=-2.1", "-2,1");

            AssertExpression("=1+1", "2");
            AssertExpression("=1+1+1", "3");
            AssertExpression("=10000000000000+2", "10000000000002");
            AssertExpression("=1.1+2.3", "3,4");
            AssertExpression("=1.1+2.3+1.01", "4,41");

            AssertExpression("=1-1", "0");
            AssertExpression("=1-2", "-1");
            AssertExpression("=1.02-1.003", "0,017");
            AssertExpression("=3*2", "6");
            AssertExpression("=3.1*2.3", "7,13");
            AssertExpression("=4/2", "2");
            AssertExpression("=3/2", "1,5");

            AssertExpression("=LEN(\"123\")/LEN(\"1234\")", "0,75");
            AssertExpression("=IF(LEN(\"123\")/LEN(\"1234\")>1, 2, 3)", "3");
        }

        private void AssertExpression(string expression, string expected)
        {
            TransformResult result = processor.Evaluate(0, expression, null);
            Assert.Null(result.Error);
            Assert.True(expected.Equals(result.Value, StringComparison.InvariantCulture), $"{result.Value} != {expected}");
        }
    }
}
