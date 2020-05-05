using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using JsonExcelExpressions;
using DocumentCreator.Properties;

namespace DocumentCreator
{
    public class DocumentCreatorTests
    {
        [Fact]
        public void CanCreateDocument()
        {
            var wordBytes = new MemoryStream(Resources.CreateDocument_docx);
            var excelBytes = new MemoryStream(Resources.CreateDocument_xlsm);
            var payload = new DocumentPayload()
            {
                Sources = new List<EvaluationSource>()
                {
                    new EvaluationSource() { Name = "RQ", Payload = JObject.Parse(Resources.CreateDocument_json) }
                }
            };

            var processor = new DocumentProcessor(null, null);
            var docStream = processor.CreateDocument(wordBytes, excelBytes, payload);

            
            Assert.NotEqual(0, docStream.Length);
            using FileStream output = File.OpenWrite("./Output/CreateDocumentTest.docx");
            docStream.CopyTo(output);
        }

        
    }
}
