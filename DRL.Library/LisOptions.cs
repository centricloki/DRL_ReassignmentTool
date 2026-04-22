using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DRL.Library
{
    public class LisOptions<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<SortOptions<T>> SortOptions { get; set; }
    }

    public class SortOptions<T>
    {
        public Expression<Func<T, object>> SortProperty { get; set; }
        public bool IsDescending { get; set; }
    }
}