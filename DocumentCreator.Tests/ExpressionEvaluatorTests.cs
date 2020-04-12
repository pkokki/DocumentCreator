using DocumentCreator.Model;
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
            var templateFields = new TemplateProcessor().FindTemplateFields(templateBytes);

            var processor = new ExpressionEvaluator();
            var response = processor.Evaluate(request, templateFields);

            var fields = new Dictionary<string, string>();
            response.Results.ToList().ForEach(o => fields.Add(o.Name, o.Text));
            Assert.Equal(23, response.Total);
            Assert.Equal(0, response.Errors);
            Assert.Equal(DateTime.Today.ToString("d/M/yyyy"), fields["J3"]);
            Assert.Equal("ΠΡΟΘΕΣΜΙΑΚΗ ΜΕ BONUS 3 ΜΗΝΩΝ - ΑΠΟ ΕΥΡΩ 10.000", fields["J4"]);
            Assert.Equal("923456789012345", fields["J5"]);
            Assert.Equal("3", fields["J6"]);
            Assert.Equal("MONTH", fields["J7"]);
            Assert.Equal("έκαστης", fields["J8"]);
            Assert.Equal("10000", fields["J9"]);
            Assert.Equal("3", fields["J10"]);
            Assert.Equal("1", fields["J11"]);
            Assert.Equal("[]", fields["J12"]);
            Assert.Equal("0,17", fields["J16"]);
            Assert.Equal("1", fields["J17"]);
            Assert.Equal("1", fields["J19"]);
            Assert.Equal("5000", fields["J21"]);
            Assert.Equal("10000", fields["J22"]);
            Assert.Equal("Προθεσμιακή με Bonus 3 Μηνών - Από Ευρώ 10.000", fields["J23"]);
            Assert.Equal("923456789012345", fields["J24"]);
            Assert.Equal("123", fields["J25"]);

            Assert.Equal("Πρώτος προαιρετικός όρος", fields["J18"]);
            Assert.Equal("", fields["J20"]);

            Assert.Equal("['1','3']", fields["J13"]);
            Assert.Equal("['0,2','0,25']", fields["J14"]);
            Assert.Equal("['500','1000']", fields["J15"]);
        }
    }
}
