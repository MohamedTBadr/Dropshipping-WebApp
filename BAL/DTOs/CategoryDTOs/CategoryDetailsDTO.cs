using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTOs.ProductDTOs;

namespace BAL.DTOs.CategoryDTOs
{
    public class CategoryDetailsDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // list of products under this category
        public List<ProductDTO> Products { get; set; }
    }
}
