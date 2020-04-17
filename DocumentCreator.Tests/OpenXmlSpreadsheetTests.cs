using DocumentCreator.Model;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlSpreadsheetTests
    {
        [Fact]
        public void CanGetTemplateFieldExpressions()
        {
            using var ms = new MemoryStream(File.ReadAllBytes(@"./Resources/OpenXmlSpreadsheetTests001.xlsm"));
            using var doc = SpreadsheetDocument.Open(ms, false);

            var templateFieldExpressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(doc, new List<EvaluationSource>());

            Assert.NotEmpty(templateFieldExpressions);
        }

        [Fact]
        public void CanUseForwardOwnCellValues()
        {
            using var ms = new MemoryStream(File.ReadAllBytes(@"./Resources/UseForwardOwnCellValues.xlsm"));
            using var doc = SpreadsheetDocument.Open(ms, false);

            var expressions = OpenXmlSpreadsheet.GetTemplateFieldExpressions(doc, new List<EvaluationSource>());
            var processor = new ExpressionEvaluator();
            var results = processor.Evaluate(expressions, null);

            Assert.True(results.All(r => r.Error == null));
            Assert.Equal("22", results.First(o => o.Name == "F01").Text);
            Assert.Equal("2", results.First(o => o.Name == "F02").Text);
            Assert.Equal("6", results.First(o => o.Name == "F03").Text);
            Assert.Equal("16", results.First(o => o.Name == "F04").Text);
            Assert.Equal("8", results.First(o => o.Name == "F05").Text);
        }
    }
}
