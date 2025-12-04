using AutoMapper;
using BAL.Services;
using BAL.Services.Implementations;
using BAL.Services.Interfaces;
using BAL.Services.Mapping;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using AutoMapper.Extensions.Microsoft.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public static class BusineessLayerRegistrationServices
    {
        public static IServiceCollection RegistrationService(this IServiceCollection Services)
        {
            Services.AddScoped<IBrandService, BrandService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<IAttachmentService, AttachmentService>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IDropshipperService, DropshipperService>();
            Services.AddScoped<IOrderService, OrderService>();

            Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            return Services;
        }

    }
}
