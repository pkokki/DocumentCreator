using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
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

        [Fact]
        public void CanCreateDocument()
        {
            var wordBytes =  File.ReadAllBytes("./Resources/T01_637218725708848542.docx");
            var excelBytes = File.ReadAllBytes("./Resources/T01_637218725708848542_M01_637218774956694571.xlsm");
            var payload = JObject.Parse(File.ReadAllText("./Resources/T01_637218725708848542.json"));
            var processor = new TemplateProcessor();
            var docBytes = processor.CreateDocument(wordBytes, excelBytes, payload);
            Assert.NotEmpty(docBytes);
        }

        [Fact]
        public void CanTransformTemplateAndMappings()
        {
            var wordBytes = File.ReadAllBytes("./Resources/T01_637218725708848542.docx");
            var excelBytes = File.ReadAllBytes("./Resources/T01_637218725708848542_M01_637218774956694571.xlsm");
            var payload = JObject.Parse(File.ReadAllText("./Resources/T01_637218725708848542.json"));
            var processor = new TemplateProcessor();
            var transformations = processor.Transform(wordBytes, excelBytes, payload);
            Assert.NotEmpty(transformations);
            Assert.True(transformations.All(o => o.Result.Error == null));
        }
    }
}
