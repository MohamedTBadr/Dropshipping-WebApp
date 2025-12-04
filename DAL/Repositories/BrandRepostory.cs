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
    public class BrandRepostory(DropShoppingDbContext dbContext) : IBrandRepository
    {
        public async Task AddAsync(Brand brand)
        {
            await dbContext.AddAsync(brand);
        }

        public async Task DeleteAsync(Guid id)
        {
            
           var brand= await dbContext.Brands.Where(x=>x.Id==id&& !x.IsDeleted).FirstOrDefaultAsync();
            brand.IsDeleted = true;
        }

        public async Task<IEnumerable<Brand>> GetAll()
        {
           var brands= await dbContext.Brands.Where(x=>!x.IsDeleted).ToListAsync();
            return brands;
        }

        public async Task<IEnumerable<Brand>> GetBrandsByCategoryId(Guid categoryId)
        {
            var brandIds = await dbContext.Products
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .Select(p => p.BrandId)
                .Distinct()
                .ToListAsync();

            if (!brandIds.Any())
                return new List<Brand>();

            return await dbContext.Brands
                .Where(b => brandIds.Contains(b.Id) && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<Brand> GetById(Guid id)
        {
            var brand= await dbContext.Brands.Where(x=>x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            return brand;
        }

        public async Task<bool> IsExisted(Guid id)
        {
           return await dbContext.Brands.Where(x=>x.Id.Equals(id)&& !x.IsDeleted).AnyAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            dbContext.Brands.Update(brand);
            
        }
    }
}
