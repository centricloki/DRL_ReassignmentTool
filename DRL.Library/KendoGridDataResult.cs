using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace DRL.Library
{
    public class KendoGridDataResult<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
    }
    public class KendoGridRequest
    {
        public int Skip { get; set; } // Number of records to skip
        public int Take { get; set; } // Number of records to take (page size)
        public string SortColumn { get; set; } // Column to sort by
        public string SortDirection { get; set; } // asc/desc
        public Dictionary<string, string> Filters { get; set; } // Column filters
    }
}
