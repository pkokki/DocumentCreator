using DocumentCreator.Properties;
using System.IO;
using Xunit;

namespace DocumentCreator
{
    public class OpenXmlWordConverterTests
    {
        [Fact]
        public void CanConvertTemplateWithImages()
        {
            using var ms = new MemoryStream(Resources.convert_template_with_images_docx);
            var conversion = OpenXmlWordConverter.ConvertToHtml(ms, "T01_12345678");
            Assert.NotNull(conversion);
            Assert.NotNull(conversion.Html);
            Assert.Equal(2, conversion.Images.Count);
        }
    }
}
