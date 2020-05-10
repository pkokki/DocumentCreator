using DocumentCreator.Core.Model;
using DocumentCreator.Properties;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
            var bytes = new MemoryStream(Resources.open_xml_spreadsheet_tests001_xlsm);
            var info = OpenXmlSpreadsheet.GetMappingInfo(bytes, null);
            Assert.NotEmpty(info.Expressions);
        }

        [Fact]
        public void CanUseForwardOwnCellValues()
        {
            var bytes = new MemoryStream(Resources.use_forward_own_cell_values_xlsm);

            var info = OpenXmlSpreadsheet.GetMappingInfo(bytes, null);
            var processor = new MappingExpressionEvaluator();
            var results = processor.Evaluate(info.Expressions, info.Sources);

            Assert.True(results.All(r => r.Error == null));
            Assert.Equal("22", results.First(o => o.Name == "F01").Text);
            Assert.Equal("2", results.First(o => o.Name == "F02").Text);
            Assert.Equal("6", results.First(o => o.Name == "F03").Text);
            Assert.Equal("16", results.First(o => o.Name == "F04").Text);
            Assert.Equal("8", results.First(o => o.Name == "F05").Text);
        }

        [Fact]
        public void CanSumRangeOwnCellValues()
        {
            var info = new MappingInfo()
            {
                Expressions = new List<MappingExpression>()
                {
                    new MappingExpression() { Name = "x", Cell = "F3", Expression = "=1" },
                    new MappingExpression() { Name = "y", Cell = "F4", Expression = "=2" },
                    new MappingExpression() { Name = "z", Cell = "F5", Expression = "=SUM(F3:F4)" },
                }
            };

            var processor = new MappingExpressionEvaluator();
            var results = processor.Evaluate(info.Expressions, info.Sources);

            var z = results.First(o => o.Name == "z");
            Assert.Null(z.Error);
            Assert.Equal(3M, z.Value);
        }
    }
}
