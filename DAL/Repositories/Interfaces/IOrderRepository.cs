using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(string dropshipperId,Order order);
        Task DeleteAsync(Guid id);
        Task<Order> GetById(Guid id);

        Task<bool> IsExisted(Guid id);

        Task SaveChangesAsync();
        // For pagination
        Task<PaginatedResult<Order>> GetAll(OrderParameters parameters);
    }
}
