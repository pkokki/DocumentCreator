using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlWordProcessingTests
    {
        [Fact]
        public void CanFindTemplateFields001()
        {
            var buffer = new MemoryStream(File.ReadAllBytes("./Resources/FindTemplateFields001.docx"));

            var templateFields = OpenXmlWordProcessing.FindTemplateFields(buffer);

            Assert.Equal(7, templateFields.Count());
            Assert.Equal(new string[] {
                "FromAccountNumber",
                "FromAccountHolder",
                "ToAccountNumber",
                "Currency",
                "Amount",
                "TransactionDate",
                "TransactionTime"
            }, templateFields.Select(o => o.Name).ToArray());
        }

        [Fact]
        public void CanFindTemplateFields002()
        {
            var buffer = new MemoryStream(File.ReadAllBytes("./Resources/FindTemplateFields002.docx"));

            var templateFields = OpenXmlWordProcessing.FindTemplateFields(buffer);

            Assert.Equal(11, templateFields.Count());
        }

    }
}
