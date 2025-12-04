using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Guid id);
        Task<Brand> GetById(Guid id);
        Task<bool> IsExisted(Guid id);
        Task<IEnumerable<Brand>> GetAll();
        Task<IEnumerable<Brand>> GetBrandsByCategoryId(Guid categoryId);

    }
}
