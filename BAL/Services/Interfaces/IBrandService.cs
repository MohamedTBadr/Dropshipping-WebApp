using BAL.DTOs.BrandDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IBrandService
    {

        Task AddAsync(BrandCreateDTO createDTO);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<BrandDTO>> GetAllAsync();
        Task<IEnumerable<BrandDTO>> GetBrandsByCategoryId(Guid categoryId);

        Task<BrandDTO> GetByIdAsync(Guid id);

        Task UpdateAsync(BrandUpdateDTO updateDTO);
        

    }
}
