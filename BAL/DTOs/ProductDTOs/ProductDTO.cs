using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.ProductDTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public Guid CategoryId { get; set; }
        public string BrandName { get; set; }
        public List<string> Images { get; set; }
        public int ModelYear { get; set; }
        public string Description { get; set; }
    }
}
