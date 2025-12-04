using System;
using System.Collections.Generic;

namespace BAL.DTOs
{
    public class PaginatedResult<T> where T : class
    {
        public IEnumerable<T> Result { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PaginatedResult() { }

        public PaginatedResult(IEnumerable<T> result, int totalCount, int pageIndex, int pageSize)
        {
            Result = result;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
