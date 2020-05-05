using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace DocumentCreator
{
    public abstract class IRepositoryTests
    {
        private const int ANY_SIZE = -1;
        private static readonly DateTime MIN_TIMESTAMP = DateTime.Today;

        public IRepositoryTests()
        {
            Repository = CreateRepository();
        }

        protected abstract IRepository CreateRepository();

        protected IRepository Repository { get; }

        private Stream CreateZeroStream(int length)
        {
            return new MemoryStream(CreateBytes(length));
        }
        private Stream CreateStream(int length)
        {
            return new MemoryStream(CreateBytes(length));
        }
        private byte[] CreateBytes(int length)
        {
            return Enumerable.Range(1, length).Select(x => Convert.ToByte(x % 256)).ToArray();
        }

        [Fact]
        public void Repository_OK()
        {
            Assert.NotNull(Repository);
        }

        [Fact]
        public async Task CreateTemplate_OK()
        {
            var result = await Repository.CreateTemplate("T1001", CreateStream(42));
            AssertTemplate(result, CreateBytes(42));
        }

        [Fact]
        public async Task CreateTemplateVersions_OK()
        {
            var v1 = await Repository.CreateTemplate("T1002", CreateStream(2));
            var v2 = await Repository.CreateTemplate("T1002", CreateStream(3));

            AssertTemplate(v1, CreateBytes(2));
            AssertTemplate(v2, CreateBytes(3));
            var versions = Repository.GetTemplateVersions("T1002");
            Assert.Equal(2, versions.Count());
        }
        [Fact]
        public async Task CreateTemplate_InvalidParameters_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Repository.CreateTemplate(null, CreateZeroStream(42)));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Repository.CreateTemplate("XXX", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Repository.CreateTemplate("XXX", new MemoryStream()));
        }

        [Fact]
        public async Task GetLatestTemplate_OK()
        {
            await Repository.CreateTemplate("T1003", CreateZeroStream(2));
            await Repository.CreateTemplate("T1003", CreateStream(3));

            var result = Repository.GetLatestTemplate("T1003");
            AssertTemplate(result, CreateBytes(3));
        }
        [Fact]
        public void GetLatestTemplate_NotExists_Null()
        {
            Assert.Null(Repository.GetLatestTemplate("XXX"));
        }
        [Fact]
        public void GetLatestTemplate_NullTemplateName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestTemplate(null));
        }


        [Fact]
        public async Task GetTemplate_OK()
        {
            await Repository.CreateTemplate("T1004", CreateZeroStream(2));
            await Repository.CreateTemplate("T1004", CreateStream(3));

            var result = Repository.GetTemplate("T1004");
            AssertTemplate(result, CreateBytes(3));
        }
        [Fact]
        public void GetTemplate_NotExists_Null()
        {
            Assert.Null(Repository.GetTemplate("XXX"));
        }
        [Fact]
        public void GetTemplate_NullTemplateName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetTemplate(null));
        }
        [Fact]
        public async Task GetTemplateVersion_OK()
        {
            var v1 = await Repository.CreateTemplate("T1004A", CreateStream(2));
            var v2 = await Repository.CreateTemplate("T1004A", CreateZeroStream(3));

            var result1 = Repository.GetTemplate(v1.TemplateName, v1.TemplateVersion);
            var result2 = Repository.GetTemplate(v2.TemplateName, v2.TemplateVersion);
            AssertTemplate(result1, CreateBytes(2));
            AssertTemplate(result2, CreateBytes(3));
        }
        [Fact]
        public async Task GetTemplateVersion_NotExists_Null()
        {
            await Repository.CreateTemplate("T1004B", CreateStream(2));

            Assert.Null(Repository.GetTemplate("T1004B", "XXX"));
        }
        [Fact]
        public async Task GetTemplateVersion_NullTemplateVersion_ReturnsLatest()
        {
            await Repository.CreateTemplate("T1004C", CreateZeroStream(2));
            await Repository.CreateTemplate("T1004C", CreateStream(3));

            var result = Repository.GetTemplate("T1004C", null);
            AssertTemplate(result, CreateBytes(3));
        }

        [Fact]
        public async Task GetTemplates_OK()
        {
            await Repository.CreateTemplate("T1005A", CreateZeroStream(2));
            await Repository.CreateTemplate("T1005B", CreateZeroStream(2));
            await Repository.CreateTemplate("T1005C", CreateZeroStream(2));

            var result = Repository.GetTemplates();

            var target = result.Where(o => o.Name.StartsWith("T1005"));
            Assert.Equal(3, target.Count());
            target.ToList().ForEach(o => AssertTemplate(o, 2));
        }


        [Fact]
        public async Task CreateMapping_OK()
        {
            await Repository.CreateTemplate("T2001", CreateZeroStream(2));

            var result = await Repository.CreateMapping("T2001", "M2001", CreateStream(42));
            
            AssertMapping(result,  CreateBytes(42));
        }
        [Fact]
        public async Task CreateMapping_NotExistingTemplate_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Repository.CreateMapping("XXX", "XXX", CreateZeroStream(2)));
        }
        [Fact]
        public async Task CreateMapping_InvalidParameters_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateMapping(null, "XXX", CreateZeroStream(2)));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateMapping("XXX", null, CreateZeroStream(2)));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateMapping("XXX", "XXX", null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateMapping("XXX", "XXX", CreateZeroStream(0)));
        }

        [Fact]
        public async Task GetLatestMapping_OK()
        {
            var template = await Repository.CreateTemplate("T2002", CreateStream(2));
            await Repository.CreateMapping(template.TemplateName, "M2002", CreateStream(2));
            await Repository.CreateMapping(template.TemplateName, "M2002", CreateStream(3));

            var result = Repository.GetLatestMapping(template.TemplateName, template.TemplateVersion, "M2002");
            AssertMapping(result, CreateBytes(3));
        }
        [Fact]
        public async Task GetLatestMapping_NotExists_Null()
        {
            var template = await Repository.CreateTemplate("T2002A", CreateStream(2));
            await Repository.CreateMapping("T2002A", "M2002A", CreateStream(2));
            await Repository.CreateMapping("T2002A", "M2002A", CreateStream(3));

            Assert.Null(Repository.GetLatestMapping("T2002A", template.TemplateVersion, "XXX"));
        }
        [Fact]
        public void GetLatestMapping_NullParams_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping(null, "XXX", "XXX"));
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping("XXX", null, "XXX"));
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping("XXX", "XXX", null));
        }

        [Fact]
        public async Task GetMapping_OK()
        {
            var template = await Repository.CreateTemplate("T2003", CreateStream(2));
            await Repository.CreateMapping("T2003", "M2003", CreateStream(2));
            await Repository.CreateMapping("T2003", "M2003", CreateStream(3));

            var result = await Repository.GetMapping("T2003", template.TemplateVersion, "M2003", null);
            AssertMapping(result, CreateBytes(3));
        }
        [Fact]
        public async Task GetMapping_WithMappingversion_OK()
        {
            var template = await Repository.CreateTemplate("T2004", CreateStream(2));
            var mappingV1 = await Repository.CreateMapping("T2004", "M2004", CreateStream(2));
            await Repository.CreateMapping("T2004", "M2004", CreateStream(3));

            var result = await Repository.GetMapping("T2004", template.TemplateVersion, "M2004", mappingV1.MappingVersion);
            AssertMapping(result, CreateBytes(2));
        }
        [Fact]
        public async Task GetMapping_WithoutTemplateVersion_OK()
        {
            await Repository.CreateTemplate("T2007", CreateStream(2));
            await Repository.CreateMapping("T2007", "M2007", CreateStream(2));
            await Repository.CreateMapping("T2007", "M2007", CreateStream(3));

            var result = await Repository.GetMapping("T2007", null, "M2007", null);
            AssertMapping(result, CreateBytes(3));
        }
        [Fact]
        public async Task GetMapping_NotExists_Null()
        {
            var template = await Repository.CreateTemplate("T2005", CreateStream(2));
            var mappingV1 = await Repository.CreateMapping("T2005", "M2005", CreateStream(2));

            Assert.Null(await Repository.GetMapping("T2005", template.TemplateVersion, "XXX", null));
            Assert.Null(await Repository.GetMapping("T2005", "XXX", "M2005", null));
            Assert.Null(await Repository.GetMapping("XXX", template.TemplateVersion, "M2005", null));
            Assert.Null(await Repository.GetMapping("T2005", template.TemplateVersion, "XXX", mappingV1.MappingVersion));
            Assert.Null(await Repository.GetMapping("T2005", "XXX", "M2005", mappingV1.MappingVersion));
            Assert.Null(await Repository.GetMapping("XXX", template.TemplateVersion, "M2005", mappingV1.MappingVersion));
        }
        [Fact]
        public async Task GetMapping_NullParams_Throws()
        {
            var template = await Repository.CreateTemplate("T2006", CreateStream(2));
            var mapping = await Repository.CreateMapping("T2006", "M2006", CreateStream(2));

            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.GetMapping(null, template.TemplateVersion, "M2006", mapping.MappingVersion));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.GetMapping("T2006", template.TemplateVersion, null, mapping.MappingVersion));
        }

        [Fact]
        public async Task GetMappings_TemplateNameOnly_OK()
        {
            await Repository.CreateTemplate("T2008", CreateStream(2));
            await Repository.CreateMapping("T2008", "M2008A", CreateZeroStream(2));
            await Repository.CreateMapping("T2008", "M2008B", CreateZeroStream(3));
            await Repository.CreateMapping("T2008", "M2008C", CreateZeroStream(4));

            var result = Repository.GetMappings("T2008", null);

            Assert.Equal(3, result.Count());
            var i = 2;
            result.ToList().ForEach(o => AssertMapping(o, i++));
        }

        [Fact]
        public async Task GetMappings_TemplateAndVersion_OK()
        {
            var template = await Repository.CreateTemplate("T2009", CreateStream(2));
            await Repository.CreateMapping("T2009", "M2009A", CreateZeroStream(2));
            await Repository.CreateMapping("T2009", "M2009B", CreateZeroStream(3));

            await Repository.CreateTemplate("T2009", CreateStream(3));
            await Repository.CreateMapping("T2009", "M2009D", CreateZeroStream(4));

            var result = Repository.GetMappings("T2009", template.TemplateVersion, null);
            Assert.Equal(2, result.Count());
            var i = 2;
            result.ToList().ForEach(o => AssertMapping(o, i++));
        }

        [Fact]
        public async Task GetMappingStats_OK()
        {
            var t1 = await Repository.CreateTemplate("T2100", CreateZeroStream(1));
            var t2 = await Repository.CreateTemplate("T2101", CreateZeroStream(2));
            var t3 = await Repository.CreateTemplate("T2102", CreateZeroStream(3));

            var t1_m1 = await Repository.CreateMapping("T2100", "M2100", CreateZeroStream(11));
            var t1_m2 = await Repository.CreateMapping("T2100", "M2101", CreateZeroStream(12));
            var t2_m1 = await Repository.CreateMapping("T2101", "M2100", CreateZeroStream(21));
            var t3_m1 = await Repository.CreateMapping("T2102", "M2100", CreateZeroStream(31));

            await Repository.CreateDocument("T2100", "M2100", CreateStream(111));
            await Repository.CreateDocument("T2100", "M2100", CreateStream(111));
            await Repository.CreateDocument("T2100", "M2100", CreateStream(112));
            await Repository.CreateDocument("T2100", "M2101", CreateStream(121));
            await Repository.CreateDocument("T2101", "M2100", CreateStream(211));

            var result1 = Repository.GetMappingStats();

            var a1 = result1.First(o => o.MappingName == "M2100");
            Assert.Null(a1.TemplateName);
            Assert.Equal(3, a1.Templates);
            Assert.Equal(4, a1.Documents);
            var a2 = result1.First(o => o.MappingName == "M2101");
            Assert.Null(a2.TemplateName);
            Assert.Equal(1, a2.Templates);
            Assert.Equal(1, a2.Documents);

            var result2 = Repository.GetMappingStats("M2100");

            var b1 = result2.First(o => o.MappingName == "M2100" && o.TemplateName == "T2100");
            Assert.Equal(1, b1.Templates);
            Assert.Equal(3, b1.Documents);
            var b2 = result2.First(o => o.MappingName == "M2100" && o.TemplateName == "T2101");
            Assert.Equal(1, b2.Templates);
            Assert.Equal(1, b2.Documents);
            var b3 = result2.First(o => o.MappingName == "M2100" && o.TemplateName == "T2102");
            Assert.Equal(1, b3.Templates);
            Assert.Equal(0, b3.Documents);
        }

        [Fact]
        public async Task CreateDocument_OK()
        {
            await Repository.CreateTemplate("T3001", CreateZeroStream(2));
            await Repository.CreateMapping("T3001", "M3001", CreateZeroStream(2));

            var result = await Repository.CreateDocument("T3001", "M3001", CreateStream(42));

            AssertDocument(result, CreateBytes(42));
        }
        [Fact]
        public async Task CreateDocument_NotExistingTemplateOrMapping_Throws()
        {
            // Both missing
            await Assert.ThrowsAsync<ArgumentException>(() => Repository.CreateDocument("XXX", "XXX", CreateZeroStream(42)));
            // Template exists, mapping missing
            await Repository.CreateTemplate("T3003", CreateZeroStream(2));
            await Assert.ThrowsAsync<ArgumentException>(() => Repository.CreateDocument("T3003", "XXX", CreateZeroStream(42)));
            // Template missing, mapping exists
            await Repository.CreateMapping("T3003", "M3004", CreateZeroStream(2));
            await Assert.ThrowsAsync<ArgumentException>(() => Repository.CreateDocument("XXX", "M3004", CreateZeroStream(42)));
        }
        [Fact]
        public async Task CreateDocument_InvalidParameters_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateDocument(null, "XXX", CreateZeroStream(42)));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateDocument("XXX", null, CreateZeroStream(42)));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateDocument("XXX", "XXX", null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => Repository.CreateDocument("XXX", "XXX", CreateZeroStream(0)));
        }

        [Fact]
        public async Task GetDocument_OK()
        {
            await Repository.CreateTemplate("T3100", CreateZeroStream(2));
            await Repository.CreateMapping("T3100", "M3100", CreateZeroStream(2));
            var docId = (await Repository.CreateDocument("T3100", "M3100", CreateStream(42))).Identifier;

            var result = Repository.GetDocument(docId);

            AssertDocument(result, CreateBytes(42));
        }

        [Fact]
        public async Task GetDocument_NotExisting_Null()
        {
            await Repository.CreateTemplate("T3100", CreateZeroStream(2));
            await Repository.CreateMapping("T3100", "M3100", CreateZeroStream(2));
            var docId = (await Repository.CreateDocument("T3100", "M3100", CreateStream(42))).Identifier;

            var result = Repository.GetDocument(docId + "1");

            Assert.Null(result);
        }

        [Fact]
        public void GetDocument_NullIdentifier_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetDocument(null));
        }

        [Fact]
        public async Task GetDocuments_OK()
        {
            var t1 = await Repository.CreateTemplate("T3101A", CreateZeroStream(2));
            await Repository.CreateMapping("T3101A", "M3101", CreateZeroStream(21));
            var d1 = (await Repository.CreateDocument("T3101A", "M3101", CreateStream(41))).Identifier;
            var d2 = (await Repository.CreateDocument("T3101A", "M3101", CreateStream(42))).Identifier;

            await Repository.CreateTemplate("T3101A", CreateZeroStream(2));
            await Repository.CreateMapping("T3101A", "M3101", CreateZeroStream(22));
            var d3 = (await Repository.CreateDocument("T3101A", "M3101", CreateStream(43))).Identifier;

            var t2 = await Repository.CreateTemplate("T3101B", CreateZeroStream(3));
            var m2 = await Repository.CreateMapping("T3101B", "M3102", CreateZeroStream(31));
            var d4 = (await Repository.CreateDocument("T3101B", "M3102", CreateStream(44))).Identifier;
            var d5 = (await Repository.CreateDocument("T3101B", "M3102", CreateStream(45))).Identifier;

            var r1 = Repository.GetDocuments();
            Assert.Equal(5, r1.Where(o => o.TemplateName.StartsWith("T3101")).Count());
            r1.ToList().ForEach(o => AssertDocument(o, ANY_SIZE));

            var r2 = Repository.GetDocuments("T3101A");
            Assert.Equal(3, r2.Count());
            r2.ToList().ForEach(o => AssertDocument(o, ANY_SIZE));

            var r3 = Repository.GetDocuments("T3101A", t1.TemplateVersion);
            Assert.Equal(2, r3.Count());
            r3.ToList().ForEach(o => AssertDocument(o, ANY_SIZE));

            var r4 = Repository.GetDocuments("T3101A", null, "M3101");
            Assert.Equal(3, r4.Count());
            r4.ToList().ForEach(o => AssertDocument(o, ANY_SIZE));

            var r5 = Repository.GetDocuments("T3101B", t2.TemplateVersion, "M3102", m2.MappingVersion);
            Assert.Equal(2, r5.Count());
            r5.ToList().ForEach(o => AssertDocument(o, ANY_SIZE));

        }

        protected virtual string PathPattern => @"([a-zA-Z]:|\.)?[\\\/](?:[a-zA-Z0-9]+[\\\/])*([A-Za-z0-9_]+)\.[A-Za-z]+";
        protected virtual string TemplateNamePattern => "[A-Za-z0-9]+_[0-9]+";
        protected virtual string TemplateFilePattern => "[A-Za-z0-9]+_[0-9]+.docx";
        protected virtual string MappingNamePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+_[0-9]+";
        protected virtual string MappingFilePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+_[0-9]+.xlsm";
        protected virtual string DocumentNamePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+_[0-9]+_[0-9]+";
        protected virtual string DocumentFilePattern => "[A-Za-z0-9]+_[0-9]+_[A-Za-z0-9]+_[0-9]+_[0-9]+.docx";
        private void AssertTemplate(ContentItemSummary content, int size)
        {
            AssertContent(content, TemplateNamePattern, TemplateFilePattern, size);
        }
        private void AssertTemplate(TemplateContent content, byte[] buffer)
        {
            AssertContent(content, TemplateNamePattern, TemplateFilePattern, buffer.Length);
            var ms = content.Buffer.ToMemoryStream();
            Assert.Equal(buffer, ms.ToArray());
        }
        private void AssertMapping(ContentItemSummary content, int size)
        {
            AssertContent(content, MappingNamePattern, MappingFilePattern, size);
        }
        private void AssertMapping(MappingContent content, byte[] buffer)
        {
            AssertContent(content, MappingNamePattern, MappingFilePattern, buffer.Length);
            var ms = content.Buffer.ToMemoryStream();
            Assert.Equal(buffer, ms.ToArray());
        }
        protected void AssertDocument(ContentItemSummary content, int size)
        {
            AssertContent(content, DocumentNamePattern, DocumentFilePattern, size);
        }
        private void AssertDocument(DocumentContent content, byte[] buffer)
        {
            AssertContent(content, DocumentNamePattern, DocumentFilePattern, buffer.Length);
            var ms = content.Buffer.ToMemoryStream();
            Assert.Equal(buffer, ms.ToArray());
        }
        private void AssertContent(ContentItemSummary content, string namePattern, string filePattern, int size)
        {
            Assert.Matches(namePattern, content.Name);
            Assert.Matches(filePattern, content.FileName);
            Assert.Matches(PathPattern, content.Path);
            Assert.True(content.Path.IndexOf(content.FileName) > 1);
            if (size == ANY_SIZE)
                Assert.True(content.Size > 0);
            else
                Assert.Equal(size, content.Size);
            Assert.True(content.Timestamp >= MIN_TIMESTAMP, $"{content.Timestamp} < {MIN_TIMESTAMP}");
        }
    }
}
