using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace DocumentCreator.Tests
{
    public class TransformProcessorTests
    {
        private readonly ITestOutputHelper output;

        public TransformProcessorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CanTestTransformations()
        {
            var processor = new TransformProcessor();
            var testRequest = JObject.Parse(File.ReadAllText("./Resources/example01.json"));
            var response = processor.Test(testRequest);
            Assert.NotNull(response);
            Assert.NotEmpty(response.Results);
            foreach (var result in response.Results)
                output.WriteLine(result.ToString()); 
            Assert.True(response.Results.TrueForAll(r => r.Error == null));
        }
    }
}
