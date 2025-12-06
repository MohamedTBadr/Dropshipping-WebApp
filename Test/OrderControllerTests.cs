using API.Controllers;
using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockService;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockService = new Mock<IOrderService>();
            _controller = new OrderController(_mockService.Object);

            // Mock a user identity for Authorized actions
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreated()
        {
            // Arrange
            var dto = new OrderCreateDTO { /* fill properties */ };
            var createdOrder = new OrderDetailsDTO { Id = Guid.NewGuid() };

            _mockService.Setup(s => s.CreateOrderAsync(dto)).ReturnsAsync(createdOrder);

            // Act
            var result = await _controller.CreateOrder(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdOrder, createdResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_WithOrder()
        {
            // Arrange
            var id = Guid.NewGuid();
            var order = new OrderDetailsDTO { Id = id };

            _mockService.Setup(s => s.GetOrderByIdAsync(id)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrderById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetOrderByIdAsync(id)).ReturnsAsync((OrderDetailsDTO)null);

            // Act
            var result = await _controller.GetOrderById(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new OrderUpdateDTO { /* fill properties */ };
            _mockService.Setup(s => s.UpdateOrderAsync(id, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrder(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteOrderAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteOrder(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOk_WithPaginatedResult()
        {
            // Arrange
            var parameters = new OrderParameters();
            var paginatedResult = new PaginatedResult<OrderDetailsDTO>
            {
                Result = new List<OrderDetailsDTO>
                {
                    new OrderDetailsDTO { Id = Guid.NewGuid() }
                },
                PageIndex = 1,
                PageSize = 10,
                TotalCount = 1
            };

            _mockService.Setup(s => s.GetAllOrdersAsync(parameters)).ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetAllOrders(parameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(paginatedResult, okResult.Value);
        }
    }
}
