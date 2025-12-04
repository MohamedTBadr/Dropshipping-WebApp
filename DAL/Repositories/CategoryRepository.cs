using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CategoryRepository(DropShoppingDbContext dbContext) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
        {
            await dbContext.Categories.AddAsync(category);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category != null)
            {
                category.IsDeleted = true;
            }
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await dbContext.Categories
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Include(c => c.Products) // include products
                .ToListAsync();
        }

        public async Task<Category> GetById(Guid id)
        {
            return (await dbContext.Categories
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(x => x.Id == id))!;
        }

        public async Task<bool> IsExisted(Guid id)
        {
            return await dbContext.Categories.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public void UpdateAsync(Category category)
        {
            dbContext.Categories.Update(category);
        }
    }
}
