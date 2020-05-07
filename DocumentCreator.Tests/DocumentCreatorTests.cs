using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using DocumentCreator.Core.Model;
using Newtonsoft.Json.Linq;
using JsonExcelExpressions;
using DocumentCreator.Properties;
using Newtonsoft.Json;

namespace DocumentCreator
{
    public class DocumentCreatorTests
    {
        [Fact]
        public void CanCreateDocument()
        {
            var wordBytes = new MemoryStream(Resources.create_document_docx);
            var excelBytes = new MemoryStream(Resources.create_document_xlsm);
            var payload = new DocumentPayload()
            {
                Sources = new List<EvaluationSource>()
                {
                    new EvaluationSource() { Name = "RQ", Payload = JObject.Parse(Resources.create_document_json) }
                }
            };

            var processor = new DocumentProcessor(null, null);
            var docStream = processor.CreateDocument(wordBytes, excelBytes, payload);

            
            Assert.NotEqual(0, docStream.Length);
            using FileStream output = File.OpenWrite("./Output/CreateDocumentTest.docx");
            docStream.CopyTo(output);
        }

        [Fact]
        public void WikiExample01()
        {
            
            // Read a Word template with content controls.
            // var templateBytes = File.ReadAllBytes("./template.docx");
            // ... or uncomment the following lines to read a ready sample template
            var templateUrl = "https://github.com/pkokki/DocumentCreator/blob/0.2.0-alpha/DocumentCreator.Tests/Resources/CreateDocument.docx?raw=true";
            using var webClient = new System.Net.WebClient();
            var templateBytes = webClient.DownloadData(templateUrl);

            // Create a stream containing the Word template
            var template = new MemoryStream(templateBytes);

            // Optionally read an Excel that contains the mappings (transformations)
            //var mappingBytes = File.ReadAllBytes("./mappings.xslm");
            // ... or uncomment the following lines to read a ready sample
            var mappingsUrl = "https://github.com/pkokki/DocumentCreator/blob/0.2.0-alpha/DocumentCreator.Tests/Resources/CreateDocument.xlsm?raw=true";
            var mappingBytes = webClient.DownloadData(mappingsUrl);

            // Create a stream containing the mappings
            var mapping = new MemoryStream(mappingBytes);

            // Read the input source from a json file
            //var jsonText = File.ReadAllText("./payload.json");
            // ... or uncomment the following lines to read a ready json
            var jsonUrl = "https://github.com/pkokki/DocumentCreator/blob/0.2.0-alpha/DocumentCreator.Tests/Resources/CreateDocument.json?raw=true";
            var jsonText = webClient.DownloadString(jsonUrl);

            // Create a JObject from the input source
            var json = JObject.Parse(jsonText);

            // Create the document payload
            var payload = new DocumentPayload()
            {
                Sources = new EvaluationSource[]
                {
                    new EvaluationSource()
                    {
                        // The name corresponds to the source identifier
                        // in the mappings file
                        Name = "RQ",
                        Payload = json
                    }
                }
            };

            // Create a document processor 
            var processor = new DocumentProcessor(null, null);

            // Generate the new document 
            var document = processor.CreateDocument(template, mapping, payload);

            // Save the document
            using FileStream output = File.OpenWrite("./document.docx");
            document.CopyTo(output);
        }
    }
}
