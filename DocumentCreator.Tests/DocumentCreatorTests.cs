using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;

namespace DocumentCreator
{
    public class DocumentCreatorTests
    {
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

        
    }
}
