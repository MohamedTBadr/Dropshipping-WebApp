using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.Internal;
using BAL.DTOs;
using BAL.DTOs.BrandDTOs;
using BAL.DTOs.CategoryDTOs;
using BAL.DTOs.CustomerDTOs;
using BAL.DTOs.DropshipperDTOs;
using BAL.DTOs.OrderDTOs;
using BAL.DTOs.OrderItemDTOs;
using BAL.DTOs.ProductDTOs;
using BAL.DTOs.WalletDTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wallet = DAL.Models.Wallet;

namespace BAL.Services.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {

            //Product Mapping
            CreateMap(typeof(PaginatedResult<Product>), typeof(PaginatedResult<ProductDTO>));

            
            CreateMap<ProductCreateDTO,Product>().ReverseMap();
            CreateMap<Product,ProductUpdateDTO>().ReverseMap();
            CreateMap<Product,ProductDetailsDTO>().ForMember(dest => dest.Images, opt => opt.MapFrom<ProductImagesUrlResolver>()).ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(x => x.Brand.Name));
            CreateMap<Product, ProductDTO>()
               .ForMember(dest=>dest.CategoryName,opt=>opt.MapFrom(x=>x.Category.Name))
               .ForMember(dest=>dest.CategoryId,opt=>opt.MapFrom(x=>x.CategoryId))
               .ForMember(dest=>dest.BrandName,opt=>opt.MapFrom(x=>x.Brand.Name))
               .ForMember(dest=>dest.ModelYear,opt=>opt.MapFrom(x=>x.ModelYear))
               .ForMember(dest=>dest.Description,opt=>opt.MapFrom(x=>x.Description))
                .ForMember(dest => dest.Images, opt => opt.MapFrom<ProductImagesUrlResolver2>());
            
            //Category Mapping
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<Category, CategoryDetailsDTO>();

            //Brand Mapping
            CreateMap<BrandCreateDTO, Brand>();
            CreateMap<BrandUpdateDTO, Brand>();
            CreateMap<Brand, BrandDTO>();

            // ✅ Safe Order Mapping (avoids null reference)
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.DropshipperName,
                opt => opt.MapFrom(src =>
                src.Dropshipper != null ? src.Dropshipper.UserName : string.Empty))
                .ReverseMap();

            CreateMap<Order, OrderDetailsDTO>()
                .ForMember(dest => dest.DropshipperName,
                           opt => opt.MapFrom(src => src.Dropshipper != null ? src.Dropshipper.UserName : string.Empty))
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
             
                .ForMember(dest => dest.CustomerAddress,
                           opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Address : string.Empty))
                .ForMember(dest => dest.CustomerPhone,
                           opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : string.Empty))
                .ReverseMap();

            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => OrderStatus.Delivering))
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ReverseMap();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();

            CreateMap<OrderItem, OrderItemsDetailsDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ReverseMap();

            CreateMap<OrderItemCreateDTO, OrderItem>().ReverseMap();



            CreateMap<DAL.Models.WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<WalletCreateDTO, Wallet>().ReverseMap();
            CreateMap<WalletCreateTransactionDTO, WalletTransactionDTO>().ReverseMap();



            CreateMap<DAL.Models.Wallet, WalletDTO>()
                .ForMember(dest => dest.WalletTransactionDTO,
                           opt => opt.MapFrom(src => src.WalletTransactions))
                .ReverseMap();



            // Customer mapping
            CreateMap<Customer, CustomerDetailsDTO>().ReverseMap();


            // Dropshipper Mapping
            CreateMap<Dropshipper, DropshipperDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // ❌ we never map password back
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore()) // handled manually
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<Dropshipper, DropshipperDetails>()
.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
.ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User.Address))
.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive))
.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
.ReverseMap();

            CreateMap<DAL.Models.WalletTransaction, BAL.DTOs.DropshipperDTOs.WalletTransaction>().ReverseMap();
            CreateMap<DAL.Models.Wallet, BAL.DTOs.DropshipperDTOs.Wallet>().ReverseMap();

        }



    }

}
