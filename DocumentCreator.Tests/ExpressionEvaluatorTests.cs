using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class ExpressionEvaluatorTests
    {
        [Fact]
        public void CanEvaluateForExcelExample01()
        {
            var json = JObject.Parse(File.ReadAllText("./Resources/EvaluateForExcelExample01.json"));
            var request = json.ToObject<EvaluationRequest>();
            var templateBytes = File.ReadAllBytes("./Resources/EvaluateForExcelExample01.docx");
            var templateFields = OpenXmlWordProcessing.FindTemplateFields(templateBytes);

            var processor = new ExpressionEvaluator();
            var response = processor.Evaluate(request, templateFields);

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
    }
}
