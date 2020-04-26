using DocumentCreator.Core.Model;
using DocumentCreator.Core.Repository;
using DocumentCreator.ExcelFormulaParser.Languages;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace DocumentCreator
{
    public static class MockData
    {
        public static DateTime Timestamp(int index)
        {
            return new DateTime(637227000000000000L + index * 1000000000000L);
        }
    }
    public class TemplateProcessorTests
    {
        private readonly Mock<IRepository> repository;
        private readonly TemplateProcessor processor;

        public TemplateProcessorTests()
        {
            repository = new Mock<IRepository>();
            processor = new TemplateProcessor(repository.Object);
        }

        [Fact]
        public void GetTemplates_NoTemplateName_OK()
        {
            var timestamp1 = MockData.Timestamp(1);
            repository.Setup(r => r.GetTemplates()).Returns(new List<ContentItemSummary>()
            {
                new ContentItemSummary() { Name = "T01_V01", FileName = "T01.docx", Path = "/files/T01.docx", Size = 42, Timestamp = timestamp1 },
                new ContentItemSummary() { Name = "T02_V02", FileName = "T02.docx", Path = "/files/T02.docx", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetTemplates();

            Assert.Equal(2, result.Count());
            var t1 = result.First();
            Assert.Equal("T01", t1.TemplateName);
            Assert.Equal("T01.docx", t1.FileName);
            Assert.Equal("V01", t1.Version);
            Assert.Equal(timestamp1, t1.Timestamp);
            Assert.Equal(42, t1.Size);
        }

        [Fact]
        public void GetTemplates_ExistingTemplateName_OK()
        {
            var timestamp1 = MockData.Timestamp(1);
            repository.Setup(r => r.GetTemplateVersions("T01")).Returns(new List<ContentItemSummary>()
            {
                new ContentItemSummary() { Name = "T01_V01", FileName = "T01A.docx", Path = "/files/T01A.docx", Size = 42, Timestamp = timestamp1 },
                new ContentItemSummary() { Name = "T01_V02", FileName = "T01B.docx", Path = "/files/T01B.docx", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetTemplates("T01");

            Assert.Equal(2, result.Count());
            var t1 = result.First();
            Assert.Equal("T01", t1.TemplateName);
            Assert.Equal("T01A.docx", t1.FileName);
            Assert.Equal("V01", t1.Version);
            Assert.Equal(timestamp1, t1.Timestamp);
            Assert.Equal(42, t1.Size);
        }

        [Fact]
        public void GetTemplates_NotExistingTemplateName_Empty()
        {
            repository.Setup(r => r.GetTemplateVersions("XXX")).Returns(new List<ContentItemSummary>());

            var result = processor.GetTemplates("XXX");

            Assert.Empty(result);
        }

        [Fact]
        public void GetTemplate_TemplateNameOnly_OK()
        {
            repository.Setup(r => r.GetTemplate("T01", null)).Returns(new ContentItem() 
            {
                Name = "T01_V01",
                FileName = "T01A.docx",
                Path = "/files/T01A.docx",
                Size = 42,
                Timestamp = MockData.Timestamp(1),
                Buffer = File.ReadAllBytes("./Resources/FindTemplateFields001.docx")
            });

            var result = processor.GetTemplate("T01");

            Assert.NotNull(result);
            Assert.Equal("T01", result.TemplateName);
            Assert.Equal("T01A.docx", result.FileName);
            Assert.Equal("V01", result.Version);
            Assert.Equal(MockData.Timestamp(1), result.Timestamp);
            Assert.Equal(42, result.Size);
            Assert.NotEmpty(result.Buffer);
            Assert.NotEmpty(result.Fields);
        }

        [Fact]
        public void GetTemplate_TemplateNameAndVersion_OK()
        {
            repository.Setup(r => r.GetTemplate("T01", "V01")).Returns(new ContentItem()
            {
                Name = "T01_V01",
                FileName = "T01A.docx",
                Path = "/files/T01A.docx",
                Size = 42,
                Timestamp = MockData.Timestamp(1),
                Buffer = File.ReadAllBytes("./Resources/FindTemplateFields001.docx")
            });

            var result = processor.GetTemplate("T01", "V01");

            Assert.NotNull(result);
            Assert.Equal("T01", result.TemplateName);
            Assert.Equal("T01A.docx", result.FileName);
            Assert.Equal("V01", result.Version);
            Assert.Equal(MockData.Timestamp(1), result.Timestamp);
            Assert.Equal(42, result.Size);
            Assert.NotEmpty(result.Buffer);
            Assert.NotEmpty(result.Fields);
        }

        [Fact]
        public void GetTemplate_NotExistingTemplateName_Null()
        {
            repository.Setup(r => r.GetTemplate("XXX", null)).Returns((ContentItem)null);

            var result = processor.GetTemplate("XXX");

            Assert.Null(result);
        }

        [Fact]
        public void GetTemplate_NotExistingTemplateVersion_Null()
        {
            repository.Setup(r => r.GetTemplate("T01", "XXX")).Returns((ContentItem)null);

            var result = processor.GetTemplate("T01", "XXX");

            Assert.Null(result);
        }

        [Fact]
        public void CreateTemplate_OK()
        {
            var templateData = new TemplateData() { TemplateName = "T01" };
            repository.Setup(r => r.CreateTemplate("T01", It.IsAny<byte[]>())).Returns((string _, byte[] bytes) => new ContentItem() 
            {
                Name = "T01_V01",
                FileName = "T01A.docx",
                Path = "/files/T01A.docx",
                Size = 42,
                Timestamp = MockData.Timestamp(1),
                Buffer = bytes
            });

            var result = processor.CreateTemplate(templateData, File.ReadAllBytes("./Resources/FindTemplateFields001.docx"));

            Assert.NotNull(result);
            Assert.Equal("T01", result.TemplateName);
            Assert.Equal("T01A.docx", result.FileName);
            Assert.Equal("V01", result.Version);
            Assert.Equal(MockData.Timestamp(1), result.Timestamp);
            Assert.Equal(42, result.Size);
            Assert.NotEmpty(result.Buffer);
            Assert.NotEmpty(result.Fields);
        }

        [Fact]
        public void CreateTemplate_NoTemplateData_Fails()
        {
            Assert.Throws<ArgumentNullException>(() => processor.CreateTemplate(null, File.ReadAllBytes("./Resources/FindTemplateFields001.docx")));

        }

        [Fact]
        public void CreateTemplate_NoTemplateName_Fails()
        {
            var templateData = new TemplateData() { TemplateName = null };
            Assert.Throws<ArgumentNullException>(() => processor.CreateTemplate(templateData, File.ReadAllBytes("./Resources/FindTemplateFields001.docx")));
        }

        [Fact]
        public void CreateTemplate_NoTemplateBuffer_Fails()
        {
            var templateData = new TemplateData() { TemplateName = "T01" };
            Assert.Throws<ArgumentNullException>(() => processor.CreateTemplate(templateData, null));
        }

        [Fact]
        public void CreateTemplate_TemplateBufferNotWord_Fails()
        {
            var templateData = new TemplateData() { TemplateName = "T01" };
            Assert.Throws<ArgumentException>(() => processor.CreateTemplate(templateData, Encoding.ASCII.GetBytes("Not WORD")));
        }
    }
}
