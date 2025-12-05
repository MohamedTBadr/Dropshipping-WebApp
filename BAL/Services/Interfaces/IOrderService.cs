using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IOrderService
    {
       
        Task<OrderDetailsDTO> CreateOrderAsync(OrderCreateDTO createDto);

        
        Task<OrderDetailsDTO> GetOrderByIdAsync(Guid id);

        Task UpdateOrderAsync(Guid id,OrderUpdateDTO updateDto);

        Task DeleteOrderAsync(Guid id);

        Task<PaginatedResult<OrderDetailsDTO>> GetAllOrdersAsync(OrderParameters parameters);
    }
}
