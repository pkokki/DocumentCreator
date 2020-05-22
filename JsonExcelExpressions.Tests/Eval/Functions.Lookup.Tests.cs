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
            AssertExpression("=LOOKUP(9, data1)", "8", json1);
            AssertExpression("=LOOKUP(1, data1)", "#N/A", json1);
            AssertExpression("=LOOKUP(16, data1)", "14", json1);

            var json2 = JObject.Parse("{ data1: [ 2, 4, 6, \"a\", \"a\", \"b\", \"d\" ] }");
            AssertExpression("=LOOKUP(\"b\", data1)", "b", json2);
            AssertExpression("=LOOKUP(\"c\", data1)", "b", json2);
            AssertExpression("=LOOKUP(9, data1)", "6", json2);
        }

        [Fact]
        public void XLOOKUP()
        {
            var json1 = JObject.Parse("{ data2: [ [\"China\", 86], [\"India\", 91], [\"USA\", 1], [\"Brazil\", 55], [\"Greece\", 30], [\"Indonesia\", 62], [\"Mexico\", 52] ] }");
            AssertExpression("=XLOOKUP(\"Brazil\", INDEX(data2, 0, 1), INDEX(data2, 0, 2))", "55", json1);
            AssertExpression("=XLOOKUP(\"Germany\", INDEX(data2, 0, 1), INDEX(data2, 0, 2))", "#N/A", json1);

            var json2 = JObject.Parse(@"
{
    data2: [
        { country: ""China"", prefix: 86},
        { country: ""India"", prefix: 91},
        { country: ""USA"", prefix: 1},
        { country: ""Brazil"", prefix: 55},
        { country: ""Greece"", prefix: 30},
        { country: ""Indonesia"", prefix: 62},
        { country: ""Mexico"", prefix: 52},
    ]
}");
            AssertExpression("=XLOOKUP(\"Brazil\", data2.country, data2.prefix)", "55", json2);
            AssertExpression("=XLOOKUP(\"Germany\", data2.country, data2.prefix, \"00\")", "00", json2);
            AssertExpression("=XLOOKUP(\"Germany\", data2.country, data2.prefix, \"00\", -1)", "86", json2); // China is largest smaller than Germany
            AssertExpression("=XLOOKUP(\"Germany\", data2.country, data2.prefix, \"00\", 1)", "30", json2); // Greece is smallest larger than Germany
            AssertExpression("=XLOOKUP(\"*zil\", data2.country, data2.prefix, , 2)", "55", json2); // Greece is smallest larger than Germany
            AssertExpression("=XLOOKUP(\"Br??il\", data2.country, data2.prefix, , 2)", "55", json2); // Greece is smallest larger than Germany
        }

        [Fact]
        public void VLOOKUP()
        {
            var json2 = JObject.Parse(@"
{
    data2: [
        [ ""Brazil"", 55 ],
        [ ""China"", 86 ],
        [ ""Greece"", 30 ],
        [ ""India"", 91 ],
        [ ""Indonesia"", 62 ],
        [ ""Mexico"", 52 ],
        [ ""USA"", 1 ]
    ]
}");
            AssertExpression("=VLOOKUP(\"India\", data2, 2)", "91", json2);
            AssertExpression("=VLOOKUP(\"India\", data2, 2, 1)", "91", json2);
            AssertExpression("=VLOOKUP(\"India\", data2, 2, 0)", "91", json2);
            AssertExpression("=VLOOKUP(\"Germany\", data2, 2, 0)", "#N/A", json2);
            AssertExpression("=VLOOKUP(\"Malta\", data2, 2, 1)", "62", json2);
            AssertExpression("=VLOOKUP(\"*ia\", data2, 2, 0)", "91", json2);
            AssertExpression("=VLOOKUP(\"*?*\", data2, 2, 0)", "55", json2);
            AssertExpression("=VLOOKUP(\"*e?e\", data2, 2, 0)", "30", json2);
        }

        [Fact]
        public void HLOOKUP()
        {
            var json2 = JObject.Parse(@"
{
    data2: [
        [ ""Brazil"", ""China"", ""Greece"", ""India"", ""Indonesia"", ""Mexico"", ""USA"" ],
        [ 55, 86, 30, 91, 62, 52, 1 ]
    ]
}");
            AssertExpression("=HLOOKUP(\"India\", data2, 2)", "91", json2);
            AssertExpression("=HLOOKUP(\"India\", data2, 2, 1)", "91", json2);
            AssertExpression("=HLOOKUP(\"India\", data2, 2, 0)", "91", json2);
            AssertExpression("=HLOOKUP(\"Germany\", data2, 2, 0)", "#N/A", json2);
            AssertExpression("=HLOOKUP(\"Malta\", data2, 2, 1)", "62", json2);
            AssertExpression("=HLOOKUP(\"*ia\", data2, 2, 0)", "91", json2);
            AssertExpression("=HLOOKUP(\"*?*\", data2, 2, 0)", "55", json2);
            AssertExpression("=HLOOKUP(\"*e?e\", data2, 2, 0)", "30", json2);
        }

        [Fact]
        public void MATCH()
        {
            var json = JObject.Parse(@"{ data2: [ 55, 186, 230, 391, 462, 552, 999 ], x: [10, 7, 4, 1] }");
            AssertExpression("=MATCH(230, data2, 0)", "3", json);
            AssertExpression("=MATCH(231, data2, 0)", "#N/A", json);
            AssertExpression("=MATCH(55, data2, 0)", "1", json);
            AssertExpression("=MATCH(999, data2, 0)", "7", json);
            AssertExpression("=MATCH(54, data2, 0)", "#N/A", json);
            AssertExpression("=MATCH(54, data2, 1)", "1", json);
            AssertExpression("=MATCH(1000, data2, 0)", "#N/A", json);
            AssertExpression("=MATCH(1000, data2, 1)", "7", json);
            AssertExpression("=MATCH(300, data2, 1)", "3", json);
            AssertExpression("=MATCH(4, x, -1)", "3", json);
            AssertExpression("=MATCH(11, x, -1)", "#N/A", json);
            AssertExpression("=MATCH(9, x, -1)", "1", json);
            AssertExpression("=MATCH(2, x, -1)", "3", json);
            AssertExpression("=MATCH(0, x, -1)", "4", json);
        }

        [Fact]
        public void CHOOSE()
        {
            AssertExpression("=CHOOSE(1, 10, 20, 30, 40)", "10");
            AssertExpression("=CHOOSE(3, 10, 20, 30, 40)", "30");
            AssertExpression("=CHOOSE(4, 10, 20, 30, 40)", "40");
            AssertExpression("=CHOOSE(3,\"Wide\", 115, \"world\", 8)", "world");

            var json = JObject.Parse(@"
{
    x: [ 10, 20, 30],
    y: [ 100, 200, 300],
    z: [ 1000, 2000, 3000]
}");
            AssertExpression("=SUM(CHOOSE(2, x, y, z))", "600", json);

        }

        [Fact]
        public void HYPERLINK()
        {
            var result = processor.Evaluate("=HYPERLINK(\"https://www.github.com\")", null, culture);
            Assert.Null(result.Error);
            Assert.Equal("https://www.github.com", result.Text);
            Assert.Equal("https://www.github.com", ((JObject)result.Value)["url"].ToString());
            Assert.Equal("https://www.github.com", ((JObject)result.Value)["text"].ToString());

            result = processor.Evaluate("=HYPERLINK(\"https://www.github.com\", \"GitHub is how people build software\")", null, culture);
            Assert.Null(result.Error);
            Assert.Equal("https://www.github.com", result.Text);
            Assert.Equal("https://www.github.com", ((JObject)result.Value)["url"].ToString());
            Assert.Equal("GitHub is how people build software", ((JObject)result.Value)["text"].ToString());
        }

        [Fact]
        public void UNIQUE()
        {
            var json1 = JObject.Parse(@"
{
    data2: [
        { country: ""USA"", prefix: 1},
        { country: ""China"", prefix: 86},
        { country: ""China"", prefix: 86},
        { country: ""India"", prefix: 91},
        { country: ""USA"", prefix: 1},
        { country: ""Brazil"", prefix: 55},
        { country: ""Mexico"", prefix: 52},
        { country: ""Greece"", prefix: 30},
        { country: ""Indonesia"", prefix: 62},
        { country: ""India"", prefix: 91},
        { country: ""Mexico"", prefix: 52},
    ]
}");
            AssertExpression("=UNIQUE(data2.country)", "['USA','China','India','Brazil','Mexico','Greece','Indonesia']", json1);
            AssertExpression("=UNIQUE(data2.country,,1)", "['Brazil','Greece','Indonesia']", json1);
            AssertExpression("=UNIQUE(data2.country & \" \" & data2.prefix)", "['USA 1','China 86','India 91','Brazil 55','Mexico 52','Greece 30','Indonesia 62']", json1);
            AssertExpression("=UNIQUE(data2.country & 1,,1)", "['Brazil1','Greece1','Indonesia1']", json1);
            AssertExpression("=UNIQUE(2 & data2.country,,1)", "['2Brazil','2Greece','2Indonesia']", json1);
        }

        [Fact]
        public void SORTBY()
        {
            var json = JObject.Parse(@"
{
    data: [
        { country: ""USA"", prefix: 1},
        { country: ""China"", prefix: 86},
        { country: ""India"", prefix: 91},
        { country: ""Brazil"", prefix: 55},
        { country: ""Greece"", prefix: 30},
        { country: ""Indonesia"", prefix: 62},
        { country: ""Mexico"", prefix: 52},
    ]
}");
            AssertExpression("=SORTBY(data.country, data.prefix)", "['USA','Greece','Mexico','Brazil','Indonesia','China','India']", json);
            AssertExpression("=SORTBY(data.prefix, data.country)", "['55','86','30','91','62','52','1']", json);
            AssertExpression("=SORTBY(data.prefix, data.country, -1)", "['1','52','62','91','30','86','55']", json);

        }

        [Fact]
        public void INDIRECT()
        {
            var json = JObject.Parse(@"{x: 1, y2: 22, z: {a:3, b:4}}");
            AssertExpression("=INDIRECT(\"a\")", "#N/A", json);
            AssertExpression("=INDIRECT(\"x\")", "1", json);
            AssertExpression("=INDIRECT(\"y\" & (1+1))", "22", json);
        }
    }
}
