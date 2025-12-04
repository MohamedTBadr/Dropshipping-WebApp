using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        void UpdateAsync(Category category);
        Task DeleteAsync(Guid id);

        Task<bool> IsExisted(Guid id);

        Task<Category> GetById(Guid id);
        Task<IEnumerable<Category>> GetAll();
    }
}
