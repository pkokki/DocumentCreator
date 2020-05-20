using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Lookup : BaseTest
    {
        public Lookup(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ROWS()
        {
            AssertExpression("=ROWS(1)", "1");
            AssertExpression("=ROWS(\"aa\")", "#VALUE!");
            AssertExpression("=ROWS(TRUE)", "#VALUE!");

            var json1 = JObject.Parse("{ data1: [ [\"Apples\", \"Lemons\"], [\"Bananas\", \"Pears\"] ] }");
            AssertExpression("=ROWS(data1)", "2", json1);
        }

        [Fact]
        public void COLUMNS()
        {
            AssertExpression("=COLUMNS(1)", "1");
            AssertExpression("=COLUMNS(\"aa\")", "#VALUE!");

            var json1 = JObject.Parse("{ data1: [ [1, 2, 3], [4, 5, 6] ] }");
            AssertExpression("=COLUMNS(data1)", "3", json1);
        }

        [Fact]
        public void INDEX()
        {
            var json1 = JObject.Parse("{ data1: [ [\"Apples\", \"Lemons\"], [\"Bananas\", \"Pears\"] ] }");
            AssertExpression("=INDEX(data1, 0, 0)", "['['Apples','Lemons']','['Bananas','Pears']']", json1);
            AssertExpression("=INDEX(data1, 1, 0)", "['Apples','Lemons']", json1);
            AssertExpression("=INDEX(data1, 2, 2)", "Pears", json1);
            AssertExpression("=INDEX(data1, 2, 1)", "Bananas", json1);
            AssertExpression("=INDEX(data1, 0, 2)", "['Lemons','Pears']", json1);
            AssertExpression("=INDEX(data1, 1)", "Apples", json1);
            AssertExpression("=INDEX(data1, 2, )", "Bananas", json1);
            AssertExpression("=INDEX(data1,, 2 )", "Lemons", json1);
            AssertExpression("=INDEX(data1, 2, 3)", "#REF!", json1);
            AssertExpression("=INDEX(data1, 3, 2)", "#REF!", json1);
            AssertExpression("=INDEX(data1, -1, 2)", "#REF!", json1);
            AssertExpression("=INDEX(data1, 1, -1)", "#REF!", json1);
        }

        [Fact]
        public void LOOKUP()
        {
            var json1 = JObject.Parse("{ data1: [ 2, 4, 6, 8, 10, 12, 14 ] }");
            AssertExpression("=LOOKUP(10, data1)", "10", json1);
            AssertExpression("=LOOKUP(1, data1)", "2", json1);
            AssertExpression("=LOOKUP(16, data1)", "14", json1);
        }
    }
}
