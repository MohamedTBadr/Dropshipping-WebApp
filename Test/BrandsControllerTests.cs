using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTOs.BrandDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PAL.Controllers;
using Xunit;

namespace Tests.Controllers
{
    public class BrandsControllerTests
    {
        private readonly Mock<IBrandService> _mockService;
        private readonly BrandsController _controller;

        public BrandsControllerTests()
        {
            _mockService = new Mock<IBrandService>();
            _controller = new BrandsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithAllBrands_WhenNoCategoryId()
        {
            // Arrange
            var brands = new List<BrandDTO> { new BrandDTO { Id = Guid.NewGuid(), Name = "Brand1" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);

            // Act
            var result = await _controller.GetAll(null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(brands, ok.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithBrandsFilteredByCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var brands = new List<BrandDTO> { new BrandDTO { Id = Guid.NewGuid(), Name = "Brand1" } };
            _mockService.Setup(s => s.GetBrandsByCategoryId(categoryId)).ReturnsAsync(brands);

            // Act
            var result = await _controller.GetAll(categoryId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(brands, ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WithBrand()
        {
            // Arrange
            var brandId = Guid.NewGuid();
            var brand = new BrandDTO { Id = brandId, Name = "Brand1" };
            _mockService.Setup(s => s.GetByIdAsync(brandId)).ReturnsAsync(brand);

            // Act
            var result = await _controller.GetById(brandId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(brand, ok.Value);
        }

        [Fact]
        public async Task Add_ReturnsCreated()
        {
            // Arrange
            var createDto = new BrandCreateDTO { Name = "Brand1" };
            _mockService.Setup(s => s.AddAsync(createDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(createDto);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var brandId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteAsync(brandId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(brandId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            // Arrange
            var updateDto = new BrandUpdateDTO { Id = Guid.NewGuid(), Name = "BrandUpdated" };
            _mockService.Setup(s => s.UpdateAsync(updateDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
