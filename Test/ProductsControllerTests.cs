using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTOs;
using BAL.DTOs.ProductDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PAL.Controllers;
using Xunit;

namespace Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithPaginatedResult()
        {
            // Arrange
            var parameters = new ProductParamaters();
            var paginatedResult = new PaginatedResult<ProductDTO>
            {
                Result = new List<ProductDTO>
                {
                    new ProductDTO { Id = Guid.NewGuid(), Name = "Test", Price = 10, CategoryName = "Cat", BrandName = "Brand", Images = new List<string>() }
                },
                PageIndex = 1,
                PageSize = 10,
                TotalCount = 1
            };
            _mockService.Setup(s => s.GetAllProducts(It.IsAny<ProductParamaters>())).ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetAll(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(paginatedResult, okResult.Value);
        }

        [Fact]
        public async Task GetAll_WithSearchTerm_PassesToService()
        {
            // Arrange
            var parameters = new ProductParamaters { SearchTerm = "phone" };
            var expected = new PaginatedResult<ProductDTO> { Result = new List<ProductDTO>(), PageIndex = 1, PageSize = 10, TotalCount = 0 };

            _mockService.Setup(s => s.GetAllProducts(It.Is<ProductParamaters>(p => p.SearchTerm == "phone"))).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetAll(parameters);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expected, ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WithProductDetails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var details = new ProductDetailsDTO
            {
                Id = id,
                Name = "Test",
                Description = "Desc",
                CategoryName = "Cat",
                BrandName = "Brand",
                Images = new List<string>()
            };
            _mockService.Setup(s => s.GetProductById(id)).ReturnsAsync(details);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(details, okResult.Value);
        }

        [Fact]
        public async Task Add_ReturnsCreated()
        {
            // Arrange
            var createDto = new ProductCreateDTO
            {
                Name = "Test",
                Description = "Desc",
                Price = 10,
                CategoryId = Guid.NewGuid(),
                BrandId = Guid.NewGuid(),
                ProductImages = new List<IFormFile>(),
                ModelYear = 2024
            };
            _mockService.Setup(s => s.AddProduct(createDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(createDto);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            // Arrange
            var updateDto = new ProductUpdateDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "Desc",
                Price = 10,
                CategoryId = Guid.NewGuid().ToString(),
                BrandId = Guid.NewGuid().ToString(),
                ProductImages = new List<IFormFile>(),
                ModelYear = 2024
            };
            _mockService.Setup(s => s.UpdateProduct(updateDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteProduct(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}