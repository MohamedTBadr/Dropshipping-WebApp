using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.ProductDTOs
{
    public class ProductUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryId { get; set; }
        public List<IFormFile> ProductImages { get; set; }
        public string BrandId { get; set; }

        public int ModelYear { get; set; }
    }
}
