using DocumentCreator.Properties;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
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
            var buffer = new MemoryStream(Resources.find_template_fields001_docx);

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
            var buffer = new MemoryStream(Resources.find_template_fields002_docx);

            var templateFields = OpenXmlWordProcessing.FindTemplateFields(buffer);

            Assert.Equal(11, templateFields.Count());
        }

        [Fact]
        public void CanCreateCorrectThemisDocument()
        {
            using var doc = WordprocessingDocument.Open(new MemoryStream(Resources.template_themis_docx), true);
            OpenXmlWordProcessing.SetContentControlContent(doc, "txt1", "SetContentControlContent");
            OpenXmlWordProcessing.SetContentControlContent(doc, "txt2", "ProcessRepeatingSection");
            OpenXmlWordProcessing.ProcessRepeatingSection(doc, "TablePercentage",
                new Dictionary<string, IEnumerable<string>>()
                {
                        { "MnthOrd", new List<string> { "1os", "2os", "3os"} },
                        { "Prcntge", new List<string> { "10%", "20%", "30%"} },
                });
            doc.SaveAs(@".\__document_themis.docx");
        }

        [Fact]
        public void CanFindCorrectTemplateFieldsWhenAlternateContent()
        {
            var buffer = new MemoryStream(Resources.simple_receipt_template_docx);

            var templateFields = OpenXmlWordProcessing.FindTemplateFields(buffer);

            Assert.Equal(9, templateFields.Count());
        }
    }
}
