using System;
using System.Collections.Generic;
using System.Text;

namespace EA.Usage.Tracking.Client
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int Total { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}
