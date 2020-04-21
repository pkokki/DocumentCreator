using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentCreator.Model
{
    public class PagingParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; }
        public bool Descending { get; set; }
    }
}
