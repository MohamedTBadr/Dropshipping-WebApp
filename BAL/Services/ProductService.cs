using AutoMapper;
using BAL.DTOs;
using BAL.DTOs.ProductDTOs;
using BAL.Exceptions;
using BAL.Services.Interfaces;
using DAL;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class ProductService(IMapper mapper,IUnitOfWork unitOfWork,IAttachmentService attachmentService,DropShoppingDbContext _context) : IProductService
    {

        public async Task AddProduct(ProductCreateDTO product)
        {
            // 1. Save Product
            var entity = mapper.Map<Product>(product);
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            // 2. Upload and save image records
            if (product.ProductImages != null && product.ProductImages.Count > 0)
            {
                foreach (var file in product.ProductImages)
                {
                    // Upload file (to folder "Products")
                    var uploadedPath = await attachmentService.Upload(file, "Products");

                    // Create ProductImages record
                    var imageEntity = new ProductImages
                    {
                        ProductId = entity.Id,
                        Image = uploadedPath
                    };

                    _context.ProductImages.Add(imageEntity);
                }

                await _context.SaveChangesAsync();
            }
        }

        //private async Task AddProductImages(Guid productId, List<IFormFile> images)
        //{
        //    var product = await unitOfWork.ProductRepository.GetById(productId);
        //    if (product == null) return;

        //    product.Images = new List<ProductImages>();

        //    foreach (var imageFile in images)
        //    {
        //        var uploadedPath = await attachmentService.Upload(imageFile, "Products");

        //        product.Images.Add(new ProductImages
        //        {
        //            Image = uploadedPath,
        //            ProductId = productId
        //        });
        //    }

        //    await unitOfWork.SaveChangesAsync();
        //}



        public async Task DeleteProduct(Guid id)
        {
            //await productRepository.DeleteAsync()
            var flag =await unitOfWork.ProductRepository.IsExisted(id);
            if (!flag) throw new ProductNotFoundException(id);
            await unitOfWork.ProductRepository.DeleteAsync(id); 
          await  unitOfWork.SaveChangesAsync();
        }

        public async Task<PaginatedResult<ProductDTO>> GetAllProducts(ProductParamaters paramaters)
        {
          var Products= await unitOfWork.ProductRepository.GetAll(paramaters);
          var Products2= mapper.Map<PaginatedResult<ProductDTO>>(Products);
            return Products2;
        }

        public async Task<ProductDetailsDTO> GetProductById(Guid id)
        {
            var product = await unitOfWork.ProductRepository.GetById(id);
            if(product == null)
            {
                throw new ProductNotFoundException(id);
            }
           return mapper.Map<ProductDetailsDTO>(product);
        }
        //TO-DO Update Images
        public async Task UpdateProduct(ProductUpdateDTO productDTO)
        {

            var product = mapper.Map<Product>(productDTO);
            // Your existing logic (e.g., update the product)
            unitOfWork.ProductRepository.UpdateAsync(product);

            List<ProductImages> productImages = await UpdateProductImage(productDTO);

            // Add to context (no cast needed)
            await _context.ProductImages.AddRangeAsync(productImages);

            // Save changes
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<List<ProductImages>> UpdateProductImage(ProductUpdateDTO productDTO)
        {
            // Process and add images
            var productImages = new List<ProductImages>();
            if (productDTO.ProductImages != null && productDTO.ProductImages.Any())
            {
                foreach (var file in productDTO.ProductImages)
                {
                    // Validate (e.g., only images, size limits)
                    var uploadedPath = await attachmentService.Upload(file, "Products");
                    // Create ProductImages instance
                    productImages.Add(new ProductImages
                    {
                        Image = uploadedPath,  // Relative path
                        ProductId = productDTO.Id  // Assuming ProductId is in DTO
                    });
                }
            }

            return productImages;
        }
    }
}
