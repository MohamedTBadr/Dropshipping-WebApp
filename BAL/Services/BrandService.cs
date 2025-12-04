using AutoMapper;
using BAL.DTOs.BrandDTOs;
using BAL.Exceptions;
using BAL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class BrandService(IMapper mapper,IBrandRepository brandRepository,IUnitOfWork unitOfWork) : IBrandService
    {
        public async Task AddAsync(BrandCreateDTO createDTO)
        {
            await brandRepository.AddAsync(mapper.Map<Brand>(createDTO));
            await unitOfWork.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var Brand = await brandRepository.IsExisted(id);
            if(Brand ==false) throw new BrandNotFoundException(id);

            await brandRepository.DeleteAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<BrandDTO>> GetAllAsync()
        {
      
            var brands = await brandRepository.GetAll();
            return mapper.Map<IEnumerable<BrandDTO>>(brands);
        }

        public async Task<IEnumerable<BrandDTO>> GetBrandsByCategoryId(Guid categoryId)
        {
            var brands = await brandRepository.GetBrandsByCategoryId(categoryId);
            return mapper.Map<IEnumerable<BrandDTO>>(brands);
        }

        public async Task<BrandDTO> GetByIdAsync(Guid id)
        {
            var flag =await brandRepository.IsExisted(id); 
            if(flag ==false) throw new BrandNotFoundException(id);
            var brand = await brandRepository.GetById(id);
            return mapper.Map<BrandDTO>(brand);
        }

        public async Task UpdateAsync(BrandUpdateDTO updateDTO)
        {
            var Brand = await brandRepository.IsExisted(updateDTO.Id);
            if (Brand == false) throw new BrandNotFoundException(updateDTO.Id);

            await brandRepository.UpdateAsync(mapper.Map<Brand>(updateDTO));
            await unitOfWork.SaveChangesAsync();


        }
    }
}
