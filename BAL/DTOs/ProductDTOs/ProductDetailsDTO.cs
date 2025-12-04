using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.ProductDTOs
{
    public class ProductDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int ModelYear { get; set; }
        public decimal Price { get; set; }

        public List<string> Images { get; set; }
    }
}
