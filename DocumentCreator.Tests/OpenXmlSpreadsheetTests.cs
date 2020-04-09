using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlSpreadsheetTests
    {
        [Fact]
        public void GetTransformations()
        {
            using var ms = new MemoryStream(File.ReadAllBytes(@".\Resources\T01_637218725708848542_M01_637218774956694571.xlsm"));
            using var doc = SpreadsheetDocument.Open(ms, false);
            var transformations = OpenXmlSpreadsheet.GetTransformations(doc);
            Assert.NotEmpty(transformations);
        }
    }
}
