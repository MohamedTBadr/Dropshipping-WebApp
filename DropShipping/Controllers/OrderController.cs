using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ✅ Create new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
                return Unauthorized();


            dto.DropshipperId = userId;

            var result = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }

        // ✅ Get order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ✅ Update order
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.UpdateOrderAsync(dto);
            return NoContent();
        }

        // ✅ Delete order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }

        // ✅ Get all orders with pagination
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderParameters parameters)
        {
            var result = await _orderService.GetAllOrdersAsync(parameters);
            return Ok(result);
        }
    }
}
