using DocumentCreator.Model;
using DocumentCreator.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace DocumentCreator
{
    public class LocalFileRepositoryTests
    {
        private const string BASE_FOLDER = @"C:\panos\repos\DocumentCreator\DocumentCreatorAPI\";
        private readonly FileRepository repo;

        public LocalFileRepositoryTests()
        {
            repo = Directory.Exists(BASE_FOLDER) ? new FileRepository(BASE_FOLDER) : null;
        }

        [Fact]
        public void GetMappings()
        {
            if (repo != null)
            {
                var all = repo.GetMappings();
                Assert.NotEmpty(all);

                var t01 = repo.GetMappings("T01");
                Assert.NotEmpty(t01);

                var none = repo.GetMappings("xxx");
                Assert.Empty(none);
            }
        }

        [Fact]
        public void GetAllDocuments()
        {
            if (repo != null)
            {
                var response = repo.GetDocuments(new DocumentParams());
                Assert.NotNull(response);
                Assert.Equal(1, response.Page);
                Assert.Equal(10, response.PageSize);
                Assert.True(response.Total > 0);
                Assert.True(response.TotalPages > 0);
                Assert.NotEmpty(response.Results);
            }
        }

        [Fact]
        public void GetAllDocumentsWithTemplateName()
        {
            if (repo != null)
            {
                var response = repo.GetDocuments(new DocumentParams() { TemplateName = "T02" });
                Assert.NotEmpty(response.Results);
            }
        }

        [Fact]
        public void GetAllDocumentsWithOrderBy()
        {
            if (repo != null)
            {
                var response = repo.GetDocuments(new DocumentParams() { OrderBy = "templateName" });
                Assert.NotEmpty(response.Results);
            }
        }

        [Fact]
        public void GetAllDocumentsWithPaging()
        {
            if (repo != null)
            {
                var page1 = repo.GetDocuments(new DocumentParams() { PageSize = 5 });
                var page2 = repo.GetDocuments(new DocumentParams() { Page = 2, PageSize = 5 });
                Assert.Equal(1, page1.Page);
                Assert.Equal(5, page1.PageSize);
                Assert.Equal(5, page1.Results.Count());
                Assert.Equal(2, page2.Page);
                Assert.Equal(5, page2.PageSize);
                Assert.Equal(5, page2.Results.Count());
                Assert.Empty(page1.Results.Select(o => o.Id).Intersect(page2.Results.Select(o => o.Id)));
            }
        }
    }
}
