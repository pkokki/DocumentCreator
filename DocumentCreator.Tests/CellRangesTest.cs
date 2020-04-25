using DocumentCreator.Core.Model;
using DocumentCreator.ExcelFormulaParser.Languages;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class CellRangesTest
    {
        [Fact]
        public void CanUseBackwardOwnCellValues()
        {
            var expressions = new List<MappingExpression>
            {
                new MappingExpression() { Name = "F01", Cell = "J13", Expression = "=3" },
                new MappingExpression() { Name = "F02", Cell = "J14", Expression = "=4" },
                new MappingExpression() { Name = "F03", Cell = "J15", Expression = "=J13+J14" }
            };

            var processor = new ExpressionEvaluator(Language.Invariant, Language.ElGr);
            var results = processor.Evaluate(expressions, null);

            var result = results.ElementAt(2);
            Assert.Null(result.Error);
            Assert.Equal("7", result.Text);
            Assert.Equal(7M, result.Value);
        }

        [Fact]
        public void CanEvaluateMapAndGetUDF()
        {
            var json = JObject.Parse(File.ReadAllText("./Resources/CanEvaluateMapAndGetUDF.json"));
            var sources = new List<MappingSource>
            {
                new MappingSource { Name = "N3", Payload = (JObject)json["sources"][0]["payload"] },
                new MappingSource { Name = "N4", Payload = (JObject)json["sources"][1]["payload"] },
                new MappingSource { Name = "N5", Payload = (JObject)json["sources"][2]["payload"] },
            };

            var expressions = new List<MappingExpression>
            {
                new MappingExpression() { Name = "F02", Cell = "F4", Expression = "=source(\"N4\",source(\"N3\",\"RequestData.PS1016\"))" },

                new MappingExpression() { Name = "F03", Cell = "F5", Expression = "=MAPVALUE(N4,MAPVALUE(N3,\"RequestData.PS1016\"))" },
                new MappingExpression() { Name = "F04", Cell = "F6", Expression = "=MAPVALUE(N3,\"RequestData.PS1016\")" },
                new MappingExpression() { Name = "F05", Cell = "F7", Expression = "=MAPVALUE(N3,\"RequestData.PS1016\",N4)" },
                new MappingExpression() { Name = "F06", Cell = "F8", Expression = "=MAPVALUE(N3,\"LogHeader.Version\")" },
                new MappingExpression() { Name = "F10", Cell = "F12", Expression = "=MAPVALUE(N3,\"RequestData.InterestTable\")" },

                new MappingExpression() { Name = "F11", Cell = "F13", Expression = "=MAPITEM(F12,\"Period\")" },

                new MappingExpression() { Name = "F12", Cell = "F14", Expression = "=GETITEM(F12,\"Period\")*100" },
                new MappingExpression() { Name = "F14", Cell = "F16", Expression = "=MAPVALUE(N5,GETITEM(F12,\"Period\",,2))" },
                new MappingExpression() { Name = "F15", Cell = "F17", Expression = "=GETITEM(F12,\"Period\",N5)" },
                new MappingExpression() { Name = "F17", Cell = "F19", Expression = "=GETITEM(F12,\"Period\",N5,2)" },
                new MappingExpression() { Name = "F18", Cell = "F20", Expression = "=GETITEM(F12,\"Period\")" },
                new MappingExpression() { Name = "F19", Cell = "F21", Expression = "=GETITEM(F12,\"Period\",,2)" },

                new MappingExpression() { Name = "F13", Cell = "F15", Expression = "=SUM(GETLIST(F12,\"Period\"))" },
                //new MappingExpression() { Name = "F16", Cell = "F18", Expression = "{=GETLIST(F12,\"Period\")+7}" },
            };

            var processor = new ExpressionEvaluator(Language.Invariant, Language.ElGr);
            var results = processor.Evaluate(expressions, sources);

            //Assert.Equal("MM", expressions.First(o => o.Name == "F01").Result.Text);
            AssertExpression(results, "F02", "MM");

            AssertExpression(results, "F03", "MM");
            AssertExpression(results, "F04", "MONTH");
            AssertExpression(results, "F05", "MM");
            AssertExpression(results, "F06", "3.10");

            AssertExpression(results, "F10", "['{}','{}']");

            AssertExpression(results, "F11", "['1','3']");
            AssertExpression(results, "F12", "100");
            AssertExpression(results, "F14", "Three");
            AssertExpression(results, "F15", "One");
            AssertExpression(results, "F17", "Three");
            AssertExpression(results, "F18", "1");
            AssertExpression(results, "F19", "3");

            AssertExpression(results, "F13", "4");
        }

        private void AssertExpression(IEnumerable<EvaluationResult> results, string name, string expected)
        {
            var result = results.First(o => o.Name == name);
            Assert.Null(result.Error);
            Assert.Equal(expected, result.Text);
        }
    }
}
