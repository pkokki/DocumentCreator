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
            var bytes = new MemoryStream(File.ReadAllBytes(@"./Resources/OpenXmlSpreadsheetTests001.xlsm"));
            var info = OpenXmlSpreadsheet.GetMappingInfo(bytes, null);
            Assert.NotEmpty(info.Expressions);
        }

        [Fact]
        public void CanUseForwardOwnCellValues()
        {
            var bytes = new MemoryStream(File.ReadAllBytes(@"./Resources/UseForwardOwnCellValues.xlsm"));

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
    }
}
