using DocumentCreator.Core;
using DocumentCreator.Core.Model;
using DocumentCreator.Repository;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    [Trait("Category", "LocalOnly")]
    public class LocalFileRepositoryTests
    {
        private const string BASE_FOLDER = @"C:\panos\repos\DocumentCreator\DocumentCreatorAPI\";
        private readonly IDocumentProcessor docProcessor;
        private readonly IMappingProcessor mappingProcessor;

        public LocalFileRepositoryTests()
        {
            var repo = new FileRepository(BASE_FOLDER);
            mappingProcessor = new MappingProcessor(repo);
            docProcessor = new DocumentProcessor(repo, null);
        }

        [Fact]
        public void GetMappings()
        {
            var t01 = mappingProcessor.GetMappings("T01");
            Assert.NotEmpty(t01);

            var none = mappingProcessor.GetMappings("xxx");
            Assert.Empty(none);
        }

        [Fact]
        public void GetAllDocuments()
        {
            var response = docProcessor.GetDocuments(new DocumentQuery());
            Assert.NotNull(response);
            Assert.Equal(1, response.Page);
            Assert.Equal(10, response.PageSize);
            Assert.True(response.Total > 0);
            Assert.True(response.TotalPages > 0);
            Assert.NotEmpty(response.Results);
        }

        [Fact]
        public void GetAllDocumentsWithTemplateName()
        {
            var response = docProcessor.GetDocuments(new DocumentQuery() { TemplateName = "T02" });
            Assert.NotEmpty(response.Results);
        }

        [Fact]
        public void GetAllDocumentsWithOrderBy()
        {
            var response = docProcessor.GetDocuments(new DocumentQuery() { OrderBy = "templateName" });
            Assert.NotEmpty(response.Results);
        }

        [Fact]
        public void GetAllDocumentsWithPaging()
        {
            var page1 = docProcessor.GetDocuments(new DocumentQuery() { PageSize = 5 });
            var page2 = docProcessor.GetDocuments(new DocumentQuery() { Page = 2, PageSize = 5 });
            Assert.Equal(1, page1.Page);
            Assert.Equal(5, page1.PageSize);
            Assert.Equal(5, page1.Results.Count());
            Assert.Equal(2, page2.Page);
            Assert.Equal(5, page2.PageSize);
            Assert.Equal(5, page2.Results.Count());
            Assert.Empty(page1.Results.Select(o => o.DocumentId).Intersect(page2.Results.Select(o => o.DocumentId)));
        }
    }
}
