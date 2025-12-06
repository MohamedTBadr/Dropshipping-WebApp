using BAL.DTOs;
using BAL.DTOs.DropshipperDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PAL.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Controllers
{
    public class DropshipperControllerTests
    {
        private readonly Mock<IDropshipperService> _mockService;
        private readonly DropshipperController _controller;

        public DropshipperControllerTests()
        {
            _mockService = new Mock<IDropshipperService>();
            _controller = new DropshipperController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllDropshippers_ReturnsOk_WithPagedResult()
        {
            // Arrange
            var page = 1;
            var dropshippers = new PaginatedResult<DropshipperDetails>
            {
                Result = new List<DropshipperDetails>
                {
                    new DropshipperDetails { Id = "1", UserName = "DS1" }
                },
                PageIndex = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _mockService.Setup(s => s.GetAllDropshippersAsync(page))
                        .ReturnsAsync(dropshippers);

            // Act
            var result = await _controller.GetAllDropshippers(page);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dropshippers, ok.Value);
        }

        [Fact]
        public async Task GetDropshipperById_ReturnsOk_WhenExists()
        {
            // Arrange
            var id = "1";
            var dropshipper = new DropshipperDetails { Id = id, UserName = "DS1" };
            _mockService.Setup(s => s.GetDropshipperByIdAsync(id)).ReturnsAsync(dropshipper);

            // Act
            var result = await _controller.GetDropshipperById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dropshipper, ok.Value);
        }

        [Fact]
        public async Task GetDropshipperById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = "1";
            _mockService.Setup(s => s.GetDropshipperByIdAsync(id)).ReturnsAsync((DropshipperDetails)null);

            // Act
            var result = await _controller.GetDropshipperById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetDropshipperWalletById_ReturnsOk_WhenExists()
        {
            // Arrange
            var id = "1";
            var wallet = new Wallet { Balance = 100 };
            _mockService.Setup(s => s.GetDropshipperWalletByIdAsync(id)).ReturnsAsync(wallet);

            // Act
            var result = await _controller.GetDropshipperWalletById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(wallet, ok.Value);
        }

        [Fact]
        public async Task GetDropshipperWalletById_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = "1";
            _mockService.Setup(s => s.GetDropshipperWalletByIdAsync(id)).ReturnsAsync((Wallet)null);

            // Act
            var result = await _controller.GetDropshipperWalletById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateDropshipper_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var dto = new DropshipperDto { UserName = "DS1" ,Email = "ds1@example.com",Password = "Password123" ,PhoneNumber="1234567890",Address= new DAL.Models.Address{
                Street="123 Main St",City="Cityville", Country="Countryland"
            }};
            _mockService.Setup(s => s.CreateDropshipperAsync(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateDropshipper(dto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        //[Fact]
        //public async Task UpdateDropshipper_ReturnsOk_WhenSuccess()
        //{
        //    // Arrange
        //    var id = "1";
        //    var updateDto = new DropshipperUpdate {
        //        Id = id,
        //        UserName = "DS1",
        //        Email = "ds1@example.com",
        //        Password = "Password123",
        //        PhoneNumber = "1234567890",
        //        Address = new DAL.Models.Address
        //        {
        //            Street = "123 Main St",
        //            City = "Cityville",
        //            Country = "Countryland"
        //        }
        //    };
        //    var updated = new DropshipperDetails { Id = id, UserName = "Updated" };
        //    //_mockService.Setup(s => s.UpdateDropshipperAsync(It.Is<DropshipperUpdate>(d => d.Id == id)))
        //    //            .ReturnsAsync(updated);

        //    // Act
        //    var result = await _controller.UpdateDropshipper(id, updateDto);

        //    // Assert
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(updated, ok.Value);
        //}

        //[Fact]
        //public async Task UpdateDropshipper_ReturnsNotFound_WhenNotExists()
        //{
        //    // Arrange
        //    var id = "1";
        //    var updateDto = new DropshipperUpdate { UserName = "Updated" };
        //    _mockService.Setup(s => s.UpdateDropshipperAsync(It.Is<DropshipperUpdate>(d => d.Id == id)))
        //                .ReturnsAsync((DropshipperDetails)null);

        //    // Act
        //    var result = await _controller.UpdateDropshipper(id, updateDto);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}

        [Fact]
        public async Task DeleteDropshipper_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var id = "1";
            var existing = new DropshipperDetails { Id = id };
            _mockService.Setup(s => s.GetDropshipperByIdAsync(id)).ReturnsAsync(existing);
            _mockService.Setup(s => s.DeleteDropshipperAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDropshipper(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteDropshipper_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var id = "1";
            _mockService.Setup(s => s.GetDropshipperByIdAsync(id)).ReturnsAsync((DropshipperDetails)null);

            // Act
            var result = await _controller.DeleteDropshipper(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
