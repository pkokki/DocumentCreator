using DocumentFormat.OpenXml.Packaging;
using System.IO;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlWordConverterTests
    {
        [Fact]
        public void CanConvertTemplateWithImages()
        {
            using var ms = new MemoryStream(File.ReadAllBytes(@"./Resources/ConvertTemplateWithImages.docx"));
            using var doc = WordprocessingDocument.Open(ms, true);
            var conversion = OpenXmlWordConverter.ConvertToHtml(doc, "T01_12345678");
            Assert.NotNull(conversion);
            Assert.NotNull(conversion.Html);
            Assert.Equal(2, conversion.Images.Count);
        }
    }
}
