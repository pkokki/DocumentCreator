using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using DocumentCreator.Core.Repository;
using DocumentCreator.Core.Model;

namespace DocumentCreator
{
    public class MappingProcessorTests
    {
        private readonly Mock<IRepository> repository;
        private readonly MappingProcessor processor;

        public MappingProcessorTests()
        {
            repository = new Mock<IRepository>();
            processor = new MappingProcessor(repository.Object);
        }

        private void AssertMappingProperties(Mapping mapping)
        {
            Assert.NotNull(mapping.MappingName);
            Assert.NotNull(mapping.TemplateName);
            Assert.NotNull(mapping.MappingVersion);
            Assert.NotNull(mapping.TemplateVersion);
            Assert.NotEqual(new DateTime(0), mapping.Timestamp);
            Assert.NotEqual(0, mapping.Size);
            Assert.NotNull(mapping.FileName);
        }


        [Fact]
        public void CanCreateMappingForTemplate()
        {
            var emptyMapping = File.ReadAllBytes("./Resources/CreateMappingForTemplate.xlsm");
            var templateBytes = File.ReadAllBytes("./Resources/CreateMappingForTemplate.docx");

            var bytes = processor.CreateMappingForTemplate(templateBytes, emptyMapping, "T01", "M01", "http://localhost/api");

            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void GetMappingStats_NoMappingName_OK()
        {
            repository.Setup(r => r.GetMappingStats(null)).Returns(new List<ContentItemStats>() { new ContentItemStats() });

            var result = processor.GetMappingStats(null);
            Assert.NotEmpty(result);
        }
        [Fact]
        public void GetMappingStats_WithMappingName_OK()
        {
            repository.Setup(r => r.GetMappingStats("T01")).Returns(new List<ContentItemStats>() { new ContentItemStats() });

            var result = processor.GetMappingStats("T01");
            Assert.NotEmpty(result);
        }


        [Fact]
        public void GetMappings_TemplateNameAndVersion_OK()
        {
            repository.Setup(r => r.GetMappings("T01", "V01", null)).Returns(new List<ContentItemSummary>()
            {
                new ContentItemSummary() { Name = "T01_V01_M01_V03", FileName = "T01_V01_M01_V03.xlsm", Path = "/files/T01_V01_M01_V03.xlsm", Size = 42, Timestamp = MockData.Timestamp(1) },
                new ContentItemSummary() { Name = "T02_V02_M02_V03", FileName = "T02_V02_M02_V03.xlsm", Path = "/files/T02_V02_M02_V03.xlsm", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetMappings("T01", "V01");
            Assert.NotEmpty(result);
            AssertMappingProperties(result.First());
        }
        [Fact]
        public void GetMappings_All_OK()
        {
            repository.Setup(r => r.GetMappings("T01", "V01", "M01")).Returns(new List<ContentItemSummary>()
            {
                new ContentItemSummary() { Name = "T01_V01_M01_V01", FileName = "T01_V01_M01_V01.xlsm", Path = "/files/T01_V01_M01_V01.xlsm", Size = 42, Timestamp = MockData.Timestamp(1) },
                new ContentItemSummary() { Name = "T02_V02_M01_V02", FileName = "T02_V02_M01_V02.xlsm", Path = "/files/T02_V02_M01_V01.xlsm", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetMappings("T01", "V01", "M01");
            Assert.NotEmpty(result);
            AssertMappingProperties(result.First());
        }
        [Fact]
        public void GetMappings_NotExistingParams_Empty()
        {
            repository.Setup(r => r.GetMappings("XXX", "XXX", null)).Returns(new List<ContentItemSummary>());
            repository.Setup(r => r.GetMappings("XXX", "XXX", "XXX")).Returns(new List<ContentItemSummary>());

            Assert.Empty(processor.GetMappings("XXX", "XXX", null));
            Assert.Empty(processor.GetMappings("XXX", "XXX", "XXX"));
        }

    }
}
