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
            var v1Name = v1.Name.Split('_')[1];
            var v2 = await Repository.CreateTemplate("T1004A", CreateZeroStream(3));
            var v2Name = v2.Name.Split('_')[1];

            var result1 = Repository.GetTemplate("T1004A", v1Name);
            var result2 = Repository.GetTemplate("T1004A", v2Name);
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
            var templateVersion = template.Name.Split('_')[1];
            await Repository.CreateMapping("T2002", "M2002", CreateStream(2));
            await Repository.CreateMapping("T2002", "M2002", CreateStream(3));

            var result = Repository.GetLatestMapping("T2002", templateVersion, "M2002");
            AssertMapping(result, CreateBytes(3));
        }
        [Fact]
        public async Task GetLatestMapping_NotExists_Null()
        {
            var template = await Repository.CreateTemplate("T2002A", CreateStream(2));
            var templateVersion = template.Name.Split('_')[1];
            await Repository.CreateMapping("T2002A", "M2002A", CreateStream(2));
            await Repository.CreateMapping("T2002A", "M2002A", CreateStream(3));

            Assert.Null(Repository.GetLatestMapping("T2002A", templateVersion, "XXX"));
        }
        [Fact]
        public void GetLatestMapping_NullParams_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping(null, "XXX", "XXX"));
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping("XXX", null, "XXX"));
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping("XXX", "XXX", null));
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
        private void AssertTemplate(ContentItem content, byte[] buffer)
        {
            AssertContent(content, TemplateNamePattern, TemplateFilePattern, buffer);
        }
        private void AssertMapping(ContentItemSummary content, int size)
        {
            AssertContent(content, MappingNamePattern, MappingFilePattern, size);
        }
        private void AssertMapping(ContentItem content, byte[] buffer)
        {
            AssertContent(content, MappingNamePattern, MappingFilePattern, buffer);
        }
        private void AssertDocument(ContentItemSummary content, int size)
        {
            AssertContent(content, DocumentNamePattern, DocumentFilePattern, size);
        }
        private void AssertDocument(ContentItem content, byte[] buffer)
        {
            AssertContent(content, DocumentNamePattern, DocumentFilePattern, buffer);
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
        private void AssertContent(ContentItem content, string namePattern, string filePattern, byte[] buffer)
        {
            AssertContent(content, namePattern, filePattern, buffer.Length);
            var ms = content.Buffer.ToMemoryStream();
            Assert.Equal(buffer, ms.ToArray());
        }

    }
}
