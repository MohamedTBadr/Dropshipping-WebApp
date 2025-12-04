using BAL.DTOs;
using BAL.DTOs.ProductDTOs;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository(DropShoppingDbContext dbContext) : IProductRepository
    {
        public async Task AddAsync(Product product)
        {
           await dbContext.Products.AddAsync(product);
         
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await dbContext.Products.Where(x=>x.Id==id).FirstOrDefaultAsync();
            product.IsDeleted = true;

        }

         public async Task<PaginatedResult<Product>> GetAll(ProductParamaters paramaters)
        {
            var query = dbContext.Products
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

            // filters
            if (paramaters.CategoryId is not null)
                query = query.Where(x => x.CategoryId == paramaters.CategoryId);

            if (paramaters.BrandId is not null)
                query = query.Where(x => x.BrandId == paramaters.BrandId);
            if(paramaters.SearchTerm is not null) query=query.Where(x=>x.Name.Contains(paramaters.SearchTerm));
            // count before pagination
            var totalCount = await query.CountAsync();

            // apply pagination
            var result = await query.Include(x => x.Images)
            .Include(c => c.Category)
            .Include(b => b.Brand)
                .Skip((paramaters.PageIndex - 1) * paramaters.PageSize)
                .Take(paramaters.PageSize)
                .ToListAsync();

            // return paginated result
            return new PaginatedResult<Product>
            {
                PageIndex = paramaters.PageIndex,
                PageSize = paramaters.PageSize,
                TotalCount = totalCount,
                Result = result
            };

        }

        public async Task<Product> GetById(Guid id)
        {
          return  (await dbContext.Products.Where(x=>!x.IsDeleted).AsNoTracking().Include(x => x.Images).Include(c => c.Category).Include(b => b.Brand).FirstOrDefaultAsync(x => x.Id == id))!;
        }
        public async Task<bool> IsExisted(Guid id)
        {
            return await dbContext.Products.AnyAsync(x => x.Id == id);
        }

        public void UpdateAsync(Product product)
        {
          dbContext.Products.Update(product);
            
        }
    }
}
