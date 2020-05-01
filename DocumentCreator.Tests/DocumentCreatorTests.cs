using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using JsonExcelExpressions;

namespace DocumentCreator
{
    public class DocumentCreatorTests
    {
        [Fact]
        public void CanCreateDocument()
        {
            var wordBytes = new MemoryStream(File.ReadAllBytes("./Resources/CreateDocument.docx"));
            var excelBytes = new MemoryStream(File.ReadAllBytes("./Resources/CreateDocument.xlsm"));
            var payload = new DocumentPayload()
            {
                Sources = new List<EvaluationSource>()
                {
                    new EvaluationSource() { Name = "RQ", Payload = JObject.Parse(File.ReadAllText("./Resources/CreateDocument.json")) }
                }
            };

            var processor = new DocumentProcessor(null);
            var docStream = processor.CreateDocument(wordBytes, excelBytes, payload);

            
            Assert.NotEqual(0, docStream.Length);
            using FileStream output = File.OpenWrite("./Output/CreateDocumentTest.docx");
            docStream.CopyTo(output);
        }

        
    }
}
