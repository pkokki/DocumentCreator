using DocumentCreator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        [Fact]
        public void Repository_OK()
        {
            Assert.NotNull(Repository);
        }

        [Fact]
        public void CreateTemplate_OK()
        {
            var result = Repository.CreateTemplate("T1001", new byte[42]);
            AssertContentItem(result, "T1001_[0-9]+", "docx", new byte[42]);
        }

        [Fact]
        public void CreateTemplateVersions_OK()
        {
            var v1 = Repository.CreateTemplate("T1002", new byte[2]);
            var v2 = Repository.CreateTemplate("T1002", new byte[3]);
            
            AssertContentItem(v1, "T1002_[0-9]+", "docx", new byte[2]);
            AssertContentItem(v2, "T1002_[0-9]+", "docx", new byte[3]);
            Assert.NotEqual(v1.Name, v2.Name);
        }
        [Fact]
        public void CreateTemplate_InvalidParameters_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.CreateTemplate(null, new byte[42]));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateTemplate("XXX", null));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateTemplate("XXX", new byte[0]));
        }

        [Fact]
        public void GetLatestTemplate_OK()
        {
            Repository.CreateTemplate("T1003", new byte[] { 1, 2 });
            Repository.CreateTemplate("T1003", new byte[] { 3, 4, 5 });

            var result = Repository.GetLatestTemplate("T1003");
            AssertContentItem(result, "T1003_[0-9]+", "docx", new byte[] { 3, 4, 5 });
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
        public void GetTemplate_OK()
        {
            Repository.CreateTemplate("T1004", new byte[] { 1, 2 });
            Repository.CreateTemplate("T1004", new byte[] { 3, 4, 5 });

            var result = Repository.GetTemplate("T1004");
            AssertContentItem(result, "T1004_[0-9]+", "docx", new byte[] { 3, 4, 5 });
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
        public void GetTemplateVersion_OK()
        {
            var v1 = Repository.CreateTemplate("T1004A", new byte[] { 1, 2 });
            Repository.CreateTemplate("T1004A", new byte[] { 3, 4, 5 });

            var result = Repository.GetTemplate("T1004A", v1.Name.Split('_')[1]);
            AssertContentItem(result, "T1004A_[0-9]+", "docx", new byte[] { 1, 2 });
        }
        [Fact]
        public void GetTemplateVersion_NotExists_Null()
        {
            Repository.CreateTemplate("T1004B", new byte[] { 3, 4, 5 });

            Assert.Null(Repository.GetTemplate("T1004B", "XXX"));
        }
        [Fact]
        public void GetTemplateVersion_NullTemplateVersion_ReturnsLatest()
        {
            Repository.CreateTemplate("T1004C", new byte[] { 1, 2 });
            Repository.CreateTemplate("T1004C", new byte[] { 3, 4, 5 });

            var result = Repository.GetTemplate("T1004C", null);
            AssertContentItem(result, "T1004C_[0-9]+", "docx", new byte[] { 3, 4, 5 });
        }

        [Fact]
        public void GetTemplates_OK()
        {
            Repository.CreateTemplate("T1005A", new byte[2]);
            Repository.CreateTemplate("T1005B", new byte[2]);
            Repository.CreateTemplate("T1005C", new byte[2]);

            var result = Repository.GetTemplates();

            var target = result.Where(o => o.Name.StartsWith("T1005"));
            Assert.Equal(3, target.Count());
            target.ToList().ForEach(o => AssertContentItemSummary(o, "T1005[A-Za-z0-9]+_[0-9]+", "docx", 2));
        }


        [Fact]
        public void CreateMapping_OK()
        {
            Repository.CreateTemplate("T2001", new byte[2]);

            var result = Repository.CreateMapping("T2001", "M2001", new byte[42]);
            
            AssertContentItem(result, "T2001_[0-9]+_M2001_[0-9]+", "xlsm", new byte[42]);
        }
        [Fact]
        public void CreateMapping_NotExistingTemplate_Throws()
        {
            Assert.Throws<ArgumentException>(() => Repository.CreateMapping("XXX", "XXX", new byte[42]));
        }
        [Fact]
        public void CreateMapping_InvalidParameters_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.CreateMapping(null, "XXX", new byte[42]));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateMapping("XXX", null, new byte[42]));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateMapping("XXX", "XXX", null));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateMapping("XXX", "XXX", new byte[0]));
        }

        [Fact]
        public void GetLatestMapping_OK()
        {
            var templateVersion = Repository.CreateTemplate("T2002", new byte[] { 1, 2 }).Name.Split('_')[1];
            Repository.CreateMapping("T2002", "M2002", new byte[] { 1, 2 });
            var mappingVersion = Repository.CreateMapping("T2002", "M2002", new byte[] { 3, 4, 5 }).Name.Split('_')[3];

            var result = Repository.GetLatestMapping("T2002", templateVersion, "M2002");
            AssertContentItem(result, "T2002_[0-9]+_M2002_" + mappingVersion, "xlsm", new byte[] { 3, 4, 5 });
        }
        [Fact]
        public void GetLatestMapping_NotExists_Null()
        {
            var templateVersion = Repository.CreateTemplate("T2002A", new byte[] { 1, 2 }).Name.Split('_')[1];
            Repository.CreateMapping("T2002A", "M2002A", new byte[] { 1, 2 });
            Repository.CreateMapping("T2002A", "M2002A", new byte[] { 3, 4, 5 });

            Assert.Null(Repository.GetLatestMapping("T2002A", templateVersion, "XXX"));
        }
        [Fact]
        public void GetLatestMapping_NullParams_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.GetLatestMapping(null, "XXX", "XXX"));
            Assert.Throws<ArgumentException>(() => Repository.GetLatestMapping("XXX", null, "XXX"));
            Assert.Throws<ArgumentException>(() => Repository.GetLatestMapping("XXX", "XXX", null));
        }


        [Fact]
        public void CreateDocument_OK()
        {
            Repository.CreateTemplate("T3001", new byte[2]);
            Repository.CreateMapping("T3001", "M3001", new byte[2]);

            var result = Repository.CreateDocument("T3001", "M3001", new byte[42]);

            AssertContentItem(result, "T3001_[0-9]+_M3001_[0-9]+_[0-9]+", "docx", new byte[42]);
        }
        [Fact]
        public void CreateDocument_NotExistingTemplateOrMapping_Throws()
        {
            // Both missing
            Assert.Throws<ArgumentException>(() => Repository.CreateDocument("XXX", "XXX", new byte[42]));
            // Template exists, mapping missing
            Repository.CreateTemplate("T3003", new byte[2]);
            Assert.Throws<ArgumentException>(() => Repository.CreateDocument("T3003", "XXX", new byte[42]));
            // Template missing, mapping exists
            Repository.CreateMapping("T3003", "M3004", new byte[2]);
            Assert.Throws<ArgumentException>(() => Repository.CreateDocument("XXX", "M3004", new byte[42]));
        }
        [Fact]
        public void CreateDocument_InvalidParameters_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Repository.CreateDocument(null, "XXX", new byte[42]));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateDocument("XXX", null, new byte[42]));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateDocument("XXX", "XXX", null));
            Assert.Throws<ArgumentNullException>(() => Repository.CreateDocument("XXX", "XXX", new byte[0]));
        }


        
        private void AssertContentItemSummary(ContentItemSummary content, string namePattern, string extension, int size)
        {
            var filePattern = $"{namePattern}.{extension}";
            Assert.Matches(namePattern, content.Name);
            Assert.Matches(filePattern, content.FileName);
            Assert.Matches(filePattern, content.Path);
            if (size == ANY_SIZE)
                Assert.True(content.Size > 0);
            else
                Assert.Equal(size, content.Size);
            Assert.True(content.Timestamp >= MIN_TIMESTAMP, $"{content.Timestamp} < {MIN_TIMESTAMP}");
        }
        private void AssertContentItem(ContentItem content, string namePattern, string extension, byte[] buffer)
        {
            AssertContentItemSummary(content, namePattern, extension, buffer.Length);
            Assert.Equal(buffer, content.Buffer);
        }

    }
}
