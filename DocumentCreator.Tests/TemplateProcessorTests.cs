using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class TemplateProcessorTests
    {
        [Fact]
        public void CanFindTemplateFields001()
        {
            var buffer = File.ReadAllBytes("./Resources/FindTemplateFields001.docx");

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
            var buffer = File.ReadAllBytes("./Resources/FindTemplateFields002.docx");

            var templateFields = OpenXmlWordProcessing.FindTemplateFields(buffer);

            Assert.Equal(11, templateFields.Count());
        }

        [Fact]
        public void CanCreateMappingForTemplate()
        {
            var emptyMapping = File.ReadAllBytes("./Resources/CreateMappingForTemplate.xlsm");
            var templateBytes = File.ReadAllBytes("./Resources/CreateMappingForTemplate.docx");

            var processor = new MappingProcessor(null);
            var bytes = processor.CreateMappingForTemplate(emptyMapping, "T01", "M01", "http://localhost/api", templateBytes);

            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void CanCreateDocument()
        {
            var wordBytes = File.ReadAllBytes("./Resources/CreateDocument.docx");
            var excelBytes = File.ReadAllBytes("./Resources/CreateDocument.xlsm");
            var payload = new DocumentPayload()
            {
                Sources = new List<MappingSource>() 
                {
                    new MappingSource() { Name = "RQ", Payload = JObject.Parse(File.ReadAllText("./Resources/CreateDocument.json")) }
                }
            };

            var processor = new DocumentProcessor(null);
            var docBytes = processor.CreateDocument(wordBytes, excelBytes, payload);

            Assert.NotEmpty(docBytes);
            File.WriteAllBytes("./Output/CreateDocumentTest.docx", docBytes);
        }

        [Fact]
        public void CanCreateDocumentInMemory()
        {
            var wordBytes = File.ReadAllBytes("./Resources/CreateDocument.docx");
            var excelBytes = File.ReadAllBytes("./Resources/CreateDocument.xlsm");
            var payload = new DocumentPayload()
            {
                Sources = new List<MappingSource>()
                {
                    new MappingSource() { Name = "RQ", Payload = JObject.Parse(File.ReadAllText("./Resources/CreateDocument.json")) }
                }
            };
            var processor = new DocumentProcessor(null);
            var results = processor.CreateDocumentInMem(wordBytes, excelBytes, payload);

            Assert.NotEmpty(results);
            Assert.True(results.All(o => o.Error == null));
            var fields = new Dictionary<string, string>();
            results.ToList().ForEach(o => fields.Add(o.Name, o.Text));
            Assert.Equal(DateTime.Today.ToString("d/M/yyyy"), fields["F01"]);
            Assert.Equal("ΠΡΟΘΕΣΜΙΑΚΗ ΜΕ BONUS 3 ΜΗΝΩΝ - ΑΠΟ ΕΥΡΩ 10.000", fields["F02"]);
            Assert.Equal("923456789012345", fields["F03"]);
            Assert.Equal("3", fields["F04"]);
            Assert.Equal("MONTH", fields["F05"]);
            Assert.Equal("έκαστης", fields["F06"]);
            Assert.Equal("10000", fields["F07"]);
            Assert.Equal("3", fields["F08"]);
            Assert.Equal("1", fields["F09"]);
            Assert.Equal("['{}','{}']", fields["F10"]);
            Assert.Equal("0,17", fields["F14"]);
            Assert.Equal("1", fields["F15"]);
            Assert.Equal("1", fields["F17"]);
            Assert.Equal("5000", fields["F19"]);
            Assert.Equal("10000", fields["F20"]);
            Assert.Equal("Προθεσμιακή με Bonus 3 Μηνών - Από Ευρώ 10.000", fields["F21"]);
            Assert.Equal("923456789012345", fields["F22"]);
            Assert.Equal("123", fields["F23"]);

            Assert.Equal("#SHOW_CONTENT#", fields["F16"]);
            Assert.Equal("#HIDE_CONTENT#", fields["F18"]);

            Assert.Equal("['1','3']", fields["F11"]);
            Assert.Equal("['0,2','0,25']", fields["F12"]);
            Assert.Equal("['500','1000']", fields["F13"]);
        }
    }
}
