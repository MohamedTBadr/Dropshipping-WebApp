using DAL.Models;
using DAL.Repositories;
using DAL.Repositories.Implementations;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class DataAccessLayerRegistrationService
    {
     public static IServiceCollection RegistrationService(this IServiceCollection Services, IConfiguration configuration)
        {

            Services.AddDbContext<DropShoppingDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            Services.AddScoped<IBrandRepository, BrandRepostory>();
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<ICategoryRepository, CategoryRepository>();
            Services.AddScoped<IOrderRepository, OrderRepository>();
            Services.AddScoped<IDropshipperRepository, DropshipperRepository>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();


            return Services;
              
        }

    }
}
