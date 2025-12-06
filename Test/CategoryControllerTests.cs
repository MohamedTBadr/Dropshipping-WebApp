using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BAL.DTOs.CategoryDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PAL.Controllers;
using Xunit;

namespace Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithAllCategories()
        {
            // Arrange
            var categories = new List<CategoryDTO>
            {
                new CategoryDTO { Id = Guid.NewGuid(), Name = "Cat1" }
            };
            _mockService.Setup(s => s.GetAllCategories()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(categories, ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WithCategory()
        {
            // Arrange
            var id = Guid.NewGuid();
            var category = new CategoryDetailsDTO { Id = id, Name = "Cat1" };

            // Mock the service
            var _mockService = new Mock<ICategoryService>();
            _mockService.Setup(s => s.GetCategoryById(id)).ReturnsAsync(category);

            // Controller with injected mock
            var _controller = new CategoryController(_mockService.Object);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(category, ok.Value);
        }


        [Fact]
        public async Task Add_ReturnsCreated()
        {
            // Arrange
            var createDto = new CategoryCreateDTO { Name = "Cat1" };
            _mockService.Setup(s => s.AddCategory(createDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(createDto);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            // Arrange
            var updateDto = new CategoryUpdateDTO { Id = Guid.NewGuid(), Name = "CatUpdated" };
            _mockService.Setup(s => s.UpdateCategory(updateDto)).Returns(Task.CompletedTask);

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
            _mockService.Setup(s => s.DeleteCategory(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
