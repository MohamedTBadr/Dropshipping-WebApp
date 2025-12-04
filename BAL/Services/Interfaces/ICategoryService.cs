using BAL.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories();
        Task<CategoryDetailsDTO> GetCategoryById(Guid id);

        Task AddCategory(CategoryCreateDTO categoryDTO);
        Task UpdateCategory(CategoryUpdateDTO categoryDTO);

        Task DeleteCategory(Guid id);
    }
}

