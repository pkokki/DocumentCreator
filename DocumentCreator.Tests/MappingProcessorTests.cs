using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class MappingProcessorTests
    {
        [Fact]
        public void CanCreateMappingForTemplate()
        {
            var emptyMapping = File.ReadAllBytes("./Resources/CreateMappingForTemplate.xlsm");
            var templateBytes = File.ReadAllBytes("./Resources/CreateMappingForTemplate.docx");

            var processor = new MappingProcessor(null);
            var bytes = processor.CreateMappingForTemplate(emptyMapping, "T01", "M01", "http://localhost/api", templateBytes);

            Assert.NotEmpty(bytes);
        }
    }
}
