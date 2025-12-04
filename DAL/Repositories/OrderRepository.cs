using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using DAL;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DropShoppingDbContext _context;

        public OrderRepository(DropShoppingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
        }

        public async Task<Order> GetById(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Dropshipper)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> IsExisted(Guid id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        public async Task<PaginatedResult<Order>> GetAll(OrderParameters parameters)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Dropshipper)
                .AsQueryable();

            // 🕓 Filter by Date Range
            if (parameters.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= parameters.FromDate.Value.ToDateTime(TimeOnly.MinValue));

            if (parameters.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= parameters.ToDate.Value.ToDateTime(TimeOnly.MaxValue));

            // 🎯 Filter by Status
            if (!string.IsNullOrEmpty(parameters.Status) && 
                Enum.TryParse<OrderStatus>(parameters.Status, true, out var status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResult<Order>(orders, totalCount, parameters.PageIndex, parameters.PageSize);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
