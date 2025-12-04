using BAL.DTOs;
using BAL.DTOs.ProductDTOs;
using DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        void UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<Product> GetById(Guid id);

        Task<bool> IsExisted(Guid id);


        //Task<IEnumerable<Product>> GetAll();
        Task<PaginatedResult<Product>> GetAll(ProductParamaters paramaters);
    }
}
