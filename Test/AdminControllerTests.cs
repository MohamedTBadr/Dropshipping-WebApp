using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.DTOs.RegistrationDTOs;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PAL.Controllers;
using Xunit;

namespace Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null);

            _controller = new AdminController(_mockUserManager.Object);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "pass123",
                ConfirmPassword = "wrongpass"
            };

            // Act
            var result = await _controller.CreateAdmin(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Passwords do not match", badRequest.Value);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsCreated_WhenSuccess()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "pass123",
                ConfirmPassword = "pass123",
                Phone = "1234567890",
                Address_City = "City",
                Address_Country = "Country",
                Address_Street = "Street"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.CreateAdmin(dto);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsBadRequest_WhenUserManagerFails()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "pass123",
                ConfirmPassword = "pass123"
            };

            var errors = new List<IdentityError> { new IdentityError { Description = "Error" } };
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
                .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            // Act
            var result = await _controller.CreateAdmin(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequest.Value);
        }

        [Fact]
        public async Task DeleteAdmin_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";
            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeleteAdmin(userId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteAdmin_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteAdmin(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAdmin_ReturnsBadRequest_WhenUserManagerFails()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId };
            var errors = new List<IdentityError> { new IdentityError { Description = "Error" } };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

            // Act
            var result = await _controller.DeleteAdmin(userId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequest.Value);
        }

        [Fact]
        public void GetAllAdmins_ReturnsOk_WithAdmins()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "Admin1", Dropshipper = null, Address = new DAL.Models.Address() },
                new User { Id = "2", UserName = "Admin2", Dropshipper = null, Address = new DAL.Models.Address() }
            }.AsQueryable();

            _mockUserManager.Setup(u => u.Users).Returns(users);

            // Act
            var result = _controller.GetAllAdmins();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAdmins = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, returnedAdmins.Count());
        }

        //[Fact]
        //public async Task GetAdminById_ReturnsOk_WhenUserExists()
        //{
        //    // Arrange
        //    var userId = "123";
        //    var user = new User { Id = userId, UserName = "Admin", Address = new DAL.Models.Address() };
        //    _mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user);

        //    // Act
        //    var result = await _controller.GetAdminById(userId);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(userId, ((dynamic)okResult.Value).Id);
        //}

        [Fact]
        public async Task GetAdminById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";
            _mockUserManager.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetAdminById(userId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFound.Value);
        }
    }
}
