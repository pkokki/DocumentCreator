using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator.Tests
{
    public class RepeatingControlTests
    {
        [Fact]
        public void CreateTemplateTest()
        {
            var processor = new TemplateProcessor();

            // POST api/Templates
            var template = processor.CreateTemplate("template001");

            // POST api/Templates/{templateId}/Versions
            var buffer = File.ReadAllBytes("./Resources/template001.002.docx");
            processor.CreateTemplateVersion(template.Id, buffer);

            // (optional) GET api/Templates/{templateId}/Fields
            var templateFields = processor.GetTemplateFields(template.Id);
            Assert.Equal(11, templateFields.Count());
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
            //var document = processor.CreateDocument(template.Id, JObject.FromObject(new
            //{
            //    FromAccountNumber = "22222222222222",
            //    FromAccountHolder = "",
            //    ToAccountNumber = "33333333333333",
            //    Currency = "USD",
            //    Amount = 65.0,
            //    TransactionDate = new DateTime(2020, 03, 10),
            //    TransactionTime = new TimeSpan(10, 11, 0)
            //}));
            //File.WriteAllBytes("./Resources/0001.docx", document.Buffer);
            //Assert.NotNull(document);
            //Assert.True(document.Buffer.Length > 0);
        }
    }
}
