using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class PagedResults<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }
        public IEnumerable<T> Results { get; set; }
    }
}
