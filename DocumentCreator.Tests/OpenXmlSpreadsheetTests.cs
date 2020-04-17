using DocumentCreator.Model;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using System.IO;
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
    }
}
