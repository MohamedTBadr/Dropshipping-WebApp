
using AutoMapper;
using BAL;
using BAL.Services;
using BAL.Services.Interfaces;
using DAL;
using DAL.Models;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text;
//using AutoMapper.Extensions.Microsoft.DependencyInjection;

namespace DropShipping
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAutoMapper();
            builder.Services.AddControllers();
            DataAccessLayerRegistrationService.RegistrationService(builder.Services,builder.Configuration);
            BusineessLayerRegistrationServices.RegistrationService(builder.Services);
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DropShoppingDbContext>()
                .AddDefaultTokenProviders();


            
          
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
                };
            });



            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:7000", "http://127.0.0.1:5500", "http://localhost:4200") // your frontend URLs
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });





            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            // Register your dependencies
            builder.Services.AddScoped<IDropshipperService, DropshipperService>();
            builder.Services.AddScoped<IDropshipperRepository, DropshipperRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
           
            //app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
           
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
