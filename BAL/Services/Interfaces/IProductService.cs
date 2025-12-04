using BAL.DTOs;
using BAL.DTOs.ProductDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IProductService
    {
       Task<PaginatedResult<ProductDTO>> GetAllProducts(ProductParamaters paramaters);
       Task<ProductDetailsDTO> GetProductById(Guid id);


        Task AddProduct(ProductCreateDTO product);
        Task UpdateProduct(ProductUpdateDTO productDTO);

        Task DeleteProduct(Guid id);

    }
}
