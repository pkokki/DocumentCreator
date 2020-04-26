using System.Collections.Generic;

namespace DocumentCreator.Core.Model
{
    public class PagingParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; }
        public bool Descending { get; set; }
    }

    public class PagedResults<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
