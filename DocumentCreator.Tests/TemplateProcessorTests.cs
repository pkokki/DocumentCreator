using System;
using System.Linq;
using System.IO;
using Xunit;

namespace DocumentCreator.Tests
{
    public class TemplateProcessorTests
    {
        [Fact]
        public void CanReadFieldsOfTemplate001Version001()
        {
            var processor = new TemplateProcessor();
            var buffer = File.ReadAllBytes("./Resources/template001.001.docx");
            var templateFields = processor.FindTemplateFields(buffer);
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
        public void CanReadFieldsOfTemplate001Version002()
        {
            var processor = new TemplateProcessor();
            var buffer = File.ReadAllBytes("./Resources/template001.002.docx");
            var templateFields = processor.FindTemplateFields(buffer);
            Assert.Equal(7, templateFields.Count());
            Assert.Equal(new string[] {
                "FromAccountNumber",
                "FromAccountHolder",
                "ToAccountNumber",
                "Currency",
                "Amount",
                "TransactionDateTime",
                "BUN"
            }, templateFields.Select(o => o.Name).ToArray());
        }
    }
}
