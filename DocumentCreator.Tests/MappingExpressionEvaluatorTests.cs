﻿using DocumentCreator.Core.Model;
using DocumentCreator.Properties;
using JsonExcelExpressions;
using JsonExcelExpressions.Lang;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class MappingExpressionEvaluatorTests
    {
        [Fact]
        public void CanDoStandaloneEvaluation()
        {
            var input = new EvaluationInput()
            {
                Fields = new List<TemplateField>()
                {
                    new TemplateField() { Name = "a" },
                    new TemplateField() { Name = "b" },
                    new TemplateField() { Name = "c" },
                },
                Expressions = new List<MappingExpression>()
                {
                    new MappingExpression() { Name="a", Expression = "=MAPVALUE(\"INP\",\"a\")" },
                    new MappingExpression() { Name="b", Expression = "=MAPVALUE(\"INP\",\"b\")" },
                    new MappingExpression() { Name="c", Expression = "=a+b" },
                },
                Sources = new List<EvaluationSource>()
                {
                    new EvaluationSource() { Name = "INP", Payload = JObject.Parse("{a:3, b:4}") }
                }
            };

            var processor = new MappingExpressionEvaluator();
            var output = processor.Evaluate(input);

            var results = output.Results.ToList();
            Assert.Equal(3, results.Count);

            Assert.Null(results[0].Error);
            Assert.Equal("3", results[0].Text);
            Assert.Null(results[1].Error);
            Assert.Equal("4", results[1].Text);
            Assert.Null(results[2].Error);
            Assert.Equal("7", results[2].Text);
        }

        [Fact]
        public void CanEvaluateForExcelExample01()
        {
            var json = JObject.Parse(Resources.evaluate_for_excel_example01_json);
            var request = json.ToObject<EvaluationRequest>();
            var templateBytes = new MemoryStream(Resources.evaluate_for_excel_example01_docx);
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var input = new EvaluationInput()
            {
                Fields = templateFields,
                Expressions = request.Expressions,
                Sources = request.Sources
            };
            var response = processor.Evaluate(input);

            var fields = new Dictionary<string, string>();
            response.Results.ToList().ForEach(o => fields.Add(o.Name, o.Text));
            Assert.Equal(23, response.Total);
            Assert.Equal(0, response.Errors);
            Assert.Equal(DateTime.Today.ToString("d/M/yyyy"), fields["F01"]);
            Assert.Equal("ΠΡΟΘΕΣΜΙΑΚΗ ΜΕ BONUS 3 ΜΗΝΩΝ - ΑΠΟ ΕΥΡΩ 10.000", fields["F02"]);
            Assert.Equal("923456789012345", fields["F03"]);
            Assert.Equal("3", fields["F04"]);
            Assert.Equal("MONTH", fields["F05"]);
            Assert.Equal("έκαστης", fields["F06"]);
            Assert.Equal("10000", fields["F07"]);
            Assert.Equal("3", fields["F08"]);
            Assert.Equal("1", fields["F09"]);
            Assert.Equal("['{}','{}']", fields["F10"]);
            Assert.Equal("['1','3']", fields["F11"]);
            Assert.Equal("['0,2','0,25']", fields["F12"]);
            Assert.Equal("['500','1000']", fields["F13"]);
            Assert.Equal("0,17", fields["F14"]);
            Assert.Equal("1", fields["F15"]);
            Assert.Equal("Πρώτος προαιρετικός όρος", fields["F16"]);
            Assert.Equal("1", fields["F17"]);
            Assert.Equal("", fields["F18"]);
            Assert.Equal("5000", fields["F19"]);
            Assert.Equal("10000", fields["F20"]);
            Assert.Equal("Προθεσμιακή με Bonus 3 Μηνών - Από Ευρώ 10.000", fields["F21"]);
            Assert.Equal("923456789012345", fields["F22"]);
            Assert.Equal("123", fields["F23"]);
        }


        [Fact]
        public void CanUseBackwardOwnCellValues()
        {
            var expressions = new List<MappingExpression>
            {
                new MappingExpression() { Name = "F01", Cell = "J13", Expression = "=3" },
                new MappingExpression() { Name = "F02", Cell = "J14", Expression = "=4" },
                new MappingExpression() { Name = "F03", Cell = "J15", Expression = "=J13+J14" }
            };

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
            var results = processor.Evaluate(expressions, null);

            var result = results.ElementAt(2);
            Assert.Null(result.Error);
            Assert.Equal("7", result.Text);
            Assert.Equal(7.0, result.Value);
        }

        [Fact]
        public void CanEvaluateMapAndGetUDF()
        {
            var json = JObject.Parse(Resources.can_evaluate_map_and_get_udf_json);
            var sources = new List<EvaluationSource>
            {
                new EvaluationSource { Name = "N3", Payload = (JObject)json["sources"][0]["payload"] },
                new EvaluationSource { Name = "N4", Payload = (JObject)json["sources"][1]["payload"] },
                new EvaluationSource { Name = "N5", Payload = (JObject)json["sources"][2]["payload"] },
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

            var processor = new MappingExpressionEvaluator(CultureInfo.GetCultureInfo("el-GR"));
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
