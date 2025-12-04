using AutoMapper;
using BAL.DTOs.ProductDTOs;
using DAL.Models;

namespace BAL.Services.Mapping
{
    public class ProductImagesUrlResolver : IValueResolver<Product, ProductDetailsDTO, List<string>>
    {
        private readonly string _baseUrl = "https://localhost:7000/Attachments/Products/";

        public List<string> Resolve(Product source, ProductDetailsDTO destination, List<string> destMember, ResolutionContext context)
        {
            if (source.Images == null || !source.Images.Any())
                return new List<string>();

            return source.Images
                         .Select(img => $"{_baseUrl}{img.Image}")
                         .ToList();
        }
    }

}
