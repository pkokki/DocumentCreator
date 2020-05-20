using JsonExcelExpressions;
using JsonExcelExpressions.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace JsonExcelExpressions.Tests
{
    public class ExpressionEvaluatorTests
    {
        [Fact]
        public void CanDoDirectEvaluations()
        {
            var processor = new ExpressionEvaluator();
            var output = processor.Evaluate("a+b", JObject.Parse("{a:3, b:4}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal(7.0, output.Value);
            Assert.Equal("7", output.Text);
            Assert.Equal("__A1", output.Name);
        }

        [Fact]
        public void CanDoDirectEvaluationsWithFunctions()
        {
            var processor = new ExpressionEvaluator();
            var output = processor.Evaluate("CONCATENATE(UPPER(a),(b.x * b.y))", JObject.Parse("{a:'panos', b: { x: 5, y: 2}}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal("PANOS10", output.Value);
            Assert.Equal("PANOS10", output.Text);
        }

        [Fact]
        public void CanDoDirectEvaluationsWithDeepPath()
        {
            var processor = new ExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var output = processor.Evaluate("a.a1.a11 + 1.5 * a.a2.a21", JObject.Parse("{a:{ a1: {a11:3}, a2: {a21:5}}}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal(10.5, output.Value);
            Assert.Equal("10,5", output.Text);
        }
        [Fact]
        public void CanDoDirectEvaluationsWithSimpleArrays()
        {
            var processor = new ExpressionEvaluator();
            //var output = processor.Evaluate("MAPVALUE(N3,\"InterestTable\")", JObject.Parse("{a:[1, 2]}"));
            var output = processor.Evaluate("a.b", JObject.Parse("{a:{b:[1, 2]}}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal(new JArray(1, 2), output.Value);
            Assert.Equal("['1','2']", output.Text);
        }
        [Fact]
        public void CanDoDirectEvaluationsWithArrayItems()
        {
            var processor = new ExpressionEvaluator();
            var output = processor.Evaluate("10+a[1].x*2", JObject.Parse("{a:[{ x: 3, y: 5}, { x: 7, y: 11}]}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal(24.0, output.Value);
            Assert.Equal("24", output.Text);
        }
        [Fact]
        public void CanDoDirectEvaluationsWithObjectArrays()
        {
            var processor = new ExpressionEvaluator();
            //var output = processor.Evaluate("MAPVALUE(N3,\"InterestTable\")", JObject.Parse("{a:[1, 2]}"));
            var output = processor.Evaluate("a.x", JObject.Parse("{a:[{x:1}, {x:2}]}"));

            Assert.True(output.Error == null, output.Error);
            Assert.Equal(new JArray(1, 2), output.Value);
            Assert.Equal("['1','2']", output.Text);
        }


        

    }
}
