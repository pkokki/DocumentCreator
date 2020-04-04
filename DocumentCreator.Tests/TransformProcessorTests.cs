using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace DocumentCreator.Tests
{
    public class TransformProcessorTests
    {
        [Fact]
        public void CanTestTransformations()
        {
            var processor = new TransformProcessor();
            var testRequest = JObject.Parse(File.ReadAllText("./Resources/example01.json"));
            var result = processor.Test(testRequest);
            Assert.NotNull(result);
        }
    }
}
