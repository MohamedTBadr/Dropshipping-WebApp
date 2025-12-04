using AutoMapper;
using BAL.DTOs.CategoryDTOs;
using BAL.Exceptions;
using BAL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class CategoryService(IMapper mapper, IUnitOfWork unitOfWork) : ICategoryService
    {
        //public async Task AddCategory(CategoryCreateDTO categoryDTO)
        //{
        //    var category = mapper.Map<Category>(categoryDTO);
        //    await unitOfWork.CategoryRepository.AddAsync(category);
        //    await unitOfWork.SaveChangesAsync();
        //}

        public async Task AddCategory(CategoryCreateDTO categoryDTO)
        {
            var category = mapper.Map<Category>(categoryDTO);

            // setting the  timestamps
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.CategoryRepository.AddAsync(category);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategory(Guid id)
        {
            var exists = await unitOfWork.CategoryRepository.IsExisted(id);
            if (!exists)
            {
                throw new CategoryNotFoundException(id);
            }

            await unitOfWork.CategoryRepository.DeleteAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
        {
            var categories = await unitOfWork.CategoryRepository.GetAll();
            return mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDetailsDTO> GetCategoryById(Guid id)
        {
            var category = await unitOfWork.CategoryRepository.GetById(id);
            if (category == null)
            {
                throw new CategoryNotFoundException(id);
            }

            return mapper.Map<CategoryDetailsDTO>(category);
        }

        //public async Task UpdateCategory(CategoryUpdateDTO categoryDTO)
        //{
        //    var category = await unitOfWork.CategoryRepository.GetById(categoryDTO.Id);
        //    if (category == null)
        //    {
        //        throw new CategoryNotFoundException(categoryDTO.Id);
        //    }

        //    mapper.Map(categoryDTO, category);

        //    unitOfWork.CategoryRepository.UpdateAsync(category);
        //    await unitOfWork.SaveChangesAsync();
        //}

        public async Task UpdateCategory(CategoryUpdateDTO categoryDTO)
        {
            var category = await unitOfWork.CategoryRepository.GetById(categoryDTO.Id);
            if (category == null)
            {
                throw new CategoryNotFoundException(categoryDTO.Id);
            }

            mapper.Map(categoryDTO, category);

            // setting the timestamp of updated at
            category.UpdatedAt = DateTime.UtcNow;

            unitOfWork.CategoryRepository.UpdateAsync(category);
            await unitOfWork.SaveChangesAsync();
        }

    }
}

