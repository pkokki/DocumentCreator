﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Moq;
using DocumentCreator.Core.Repository;
using DocumentCreator.Core.Model;
using DocumentCreator.Properties;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using JsonExcelExpressions;

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
            var emptyMapping = new MemoryStream(Resources.create_mapping_for_template_xlsm);
            var templateBytes = new MemoryStream(Resources.create_mapping_for_template_docx);

            var info = new FillMappingInfo()
            {
                TemplateName = "T01",
                MappingName = "M01",
                TestUrl = "http://localhost/api",
                Payload = new FillMappingPayload()
                {
                    Sources = new List<EvaluationSource>() 
                    { 
                        new EvaluationSource() { Name = "S1", Payload = JObject.Parse("{x: 5, y: 6}") },
                        new EvaluationSource() { Name = "S2", Payload = JObject.Parse("{z: 10}") }
                    }
                }
            };
            var bytes = processor.CreateMappingForTemplate(templateBytes, emptyMapping, info);

            Assert.NotEqual(0, bytes.Length);
            using FileStream output = File.Open("./mappings.xlsm", FileMode.Create);
            bytes.CopyTo(output);
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
            repository.Setup(r => r.GetMappings("T01", "V01", null)).Returns(new List<MappingContentSummary>()
            {
                new MappingContentSummary() { Name = "T01_V01_M01_V03", TemplateName="T01", TemplateVersion ="V01", MappingName ="M01", MappingVersion ="V01", FileName = "T01_V01_M01_V03.xlsm", Path = "/files/T01_V01_M01_V03.xlsm", Size = 42, Timestamp = MockData.Timestamp(1) },
                new MappingContentSummary() { Name = "T02_V02_M02_V03", TemplateName="T02", TemplateVersion ="V02", MappingName ="M02", MappingVersion ="V03", FileName = "T02_V02_M02_V03.xlsm", Path = "/files/T02_V02_M02_V03.xlsm", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetMappings("T01", "V01");
            Assert.NotEmpty(result);
            AssertMappingProperties(result.First());
        }
        [Fact]
        public void GetMappings_All_OK()
        {
            repository.Setup(r => r.GetMappings("T01", "V01", "M01")).Returns(new List<MappingContentSummary>()
            {
                new MappingContentSummary() { Name = "T01_V01_M01_V01", TemplateName="T01", TemplateVersion ="V01", MappingName ="M01", MappingVersion ="V01", FileName = "T01_V01_M01_V01.xlsm", Path = "/files/T01_V01_M01_V01.xlsm", Size = 42, Timestamp = MockData.Timestamp(1) },
                new MappingContentSummary() { Name = "T02_V02_M01_V02", TemplateName="T02", TemplateVersion ="V02", MappingName ="M01", MappingVersion ="V02", FileName = "T02_V02_M01_V02.xlsm", Path = "/files/T02_V02_M01_V01.xlsm", Size = 43, Timestamp = MockData.Timestamp(2) },
            });

            var result = processor.GetMappings("T01", "V01", "M01");
            Assert.NotEmpty(result);
            AssertMappingProperties(result.First());
        }
        [Fact]
        public void GetMappings_NotExistingParams_Empty()
        {
            repository.Setup(r => r.GetMappings("XXX", "XXX", null)).Returns(new List<MappingContentSummary>());
            repository.Setup(r => r.GetMappings("XXX", "XXX", "XXX")).Returns(new List<MappingContentSummary>());

            Assert.Empty(processor.GetMappings("XXX", "XXX", null));
            Assert.Empty(processor.GetMappings("XXX", "XXX", "XXX"));
        }

        [Fact]
        public void Evaluate_NullTemplateName_OK()
        {
            var request = new EvaluationRequest()
            {
                Expressions = new List<MappingExpression>() 
                {
                    new MappingExpression()
                    {
                        Name = "E1",
                        Expression = "1+1"
                    }
                }
            };
            var output = processor.Evaluate(request);

            Assert.Equal("2", output.Results.First().Text);
        }

        [Fact]
        public void Evaluate_ReturnsCell()
        {
            var request = new EvaluationRequest()
            {
                Expressions = new List<MappingExpression>()
                {
                    new MappingExpression()
                    {
                        Name = "E1",
                        Cell = "A1",
                        Expression = "1+1"
                    }
                }
            };
            var output = processor.Evaluate(request);

            Assert.Equal("A1", output.Results.First().Cell);
        }
    }
}
