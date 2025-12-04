using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.ProductDTOs
{
    public class ProductParamaters
    {
        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
         //public int PageCount { get; set; }
    }
}
