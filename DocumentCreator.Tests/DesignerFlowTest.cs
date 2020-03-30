using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator.Tests
{
    public class DesignerFlowTest
    {
        [Fact]
        public void CreateTemplateTest()
        {
            var processor = new TemplateProcessor();

            // POST api/Templates
            var template = processor.CreateTemplate("template001");

            // POST api/Templates/{templateId}/Versions
            var buffer = File.ReadAllBytes("./Resources/template001.001.docx");
            processor.CreateTemplateVersion(template.Id, buffer);

            // (optional) GET api/Templates/{templateId}/Fields
            var templateFields = processor.GetTemplateFields(template.Id);
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

            // POST api/Templates/{templateId}/Documents
            var document = processor.CreateDocument(template.Id, JObject.FromObject(new {
                FromAccountNumber = "22222222222222",
                FromAccountHolder = "",
                ToAccountNumber = "33333333333333",
                Currency = "USD",
                Amount = 65.0,
                TransactionDate = new DateTime(2020, 03, 10),
                TransactionTime = new TimeSpan(10, 11, 0)
            }));
            File.WriteAllBytes("./Resources/0001.docx", document.Buffer);
            Assert.NotNull(document);
            Assert.True(document.Buffer.Length > 0);
        }

        [Fact]
        public void CreateTemplateWithMappings()
        {
            var processor = new TemplateProcessor();

            // POST api/Templates
            var template = processor.CreateTemplate("template001");

            // POST api/Templates/{templateId}/Versions
            var buffer = File.ReadAllBytes("./Resources/template001.001.docx");
            processor.CreateTemplateVersion(template.Id, buffer);

            // (optional) GET api/Templates/{templateId}/Fields
            // as above

            // POST api/Templates/{templateId}/Mappings/{name}
            processor.UpsertMapping(template.Id, "KEO", JObject.FromObject(new {
                FromAccountNumber = "F1",
                FromAccountHolder = "F2",
                ToAccountNumber = "F3",
                Currency = "F4",
                Amount = "F5",
                TransactionDate = "F6",
                TransactionTime = "F7"
            }));

            // POST api/Templates/{templateId}/Documents/{name}
            var document = processor.CreateDocument(template.Id, "KEO", JObject.FromObject(new
            {
                F1 = "444444444",
                F2 = "John Doe",
                F3 = "555555555555",
                F4 = "CHF",
                F5 = 165.0,
                F6 = new DateTime(2020, 04, 12),
                F7 = new TimeSpan(17, 13, 0)
            }));
            File.WriteAllBytes("./Resources/0002.docx", document.Buffer);
            Assert.NotNull(document);
            Assert.True(document.Buffer.Length > 0);
        }
    }
}
