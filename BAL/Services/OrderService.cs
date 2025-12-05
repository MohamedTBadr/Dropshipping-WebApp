using AutoMapper;
using BAL.DTOs;
using BAL.DTOs.OrderDTOs;
using BAL.DTOs.OrderItemDTOs;
using BAL.DTOs.WalletDTOs;
using BAL.Services.Interfaces;
using DAL;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly DropShoppingDbContext _context;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, DropShoppingDbContext context)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _context = context;
        }

        // ✅ Create new order by dropshipper
        public async Task<OrderDetailsDTO> CreateOrderAsync(OrderCreateDTO createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🧾 Create or get existing customer
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c =>
                        c.PhoneNumber == createDto.Customer.PhoneNumber &&
                        c.DropshipperId == createDto.DropshipperId);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        Name = createDto.Customer.Name,
                        Address = createDto.Customer.Address,
                        PhoneNumber = createDto.Customer.PhoneNumber,
                        DropshipperId = createDto.DropshipperId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync(); // Save customer first
                }

                // 🛒 Create order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customer.Id,
                    DropshipperId = createDto.DropshipperId,
                    OrderStatus = OrderStatus.Delivering,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync(); // Save order first

                // 🧮 Add order items
                var orderItems = new List<OrderItem>();
                decimal totalPrice = 0;

                foreach (var itemDto in createDto.Items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == itemDto.ProductId);
                    if (product == null)
                        throw new Exception($"Product with ID {itemDto.ProductId} not found");

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id, // ✅ Set the OrderId after order is saved
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        OrderItemDiscount = 0, // You can add discount logic later
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    totalPrice += product.Price * itemDto.Quantity;
                    orderItems.Add(orderItem);
                }

                await _context.OrderItems.AddRangeAsync(orderItems);
                
                // Update order with calculated price
                order.OrderPrice = totalPrice;
                order.OrderDiscount = 0; // Future feature
                _context.Orders.Update(order);






                // Finally, save changes


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                // Reload order with all relationships for mapping
                var savedOrder = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
                        .ThenInclude(i => i.Product)
                    .Include(o => o.Dropshipper)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                return _mapper.Map<OrderDetailsDTO>(savedOrder);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ✅ Get single order with details
        public async Task<OrderDetailsDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetById(id);
            if (order == null)
                throw new Exception("Order not found");

            return _mapper.Map<OrderDetailsDTO>(order);
        }

        // ✅ Update existing order
        public async Task UpdateOrderAsync(Guid id ,OrderUpdateDTO updateDto)
        {
            var order = await _orderRepository.GetById(id);
            if (order == null)
                throw new Exception("Order not found");

            //order.OrderPrice = updateDto.OrderPrice;
            //order.OrderDiscount = updateDto.OrderDiscount;
            
            order.OrderStatus = updateDto.OrderStatus;
            order.ShippedDate = updateDto.ShippedDate;
            order.UpdatedAt = DateTime.Now;
            var dropshipperId = order.DropshipperId;

            await _orderRepository.UpdateAsync(dropshipperId,order);
            await _orderRepository.SaveChangesAsync();
        }

        // ✅ Delete order
        public async Task DeleteOrderAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
            await _orderRepository.SaveChangesAsync();
        }

        // ✅ Get paginated orders
        public async Task<PaginatedResult<OrderDetailsDTO>> GetAllOrdersAsync(OrderParameters parameters)
        {
            var orders = await _orderRepository.GetAll(parameters);

            return new PaginatedResult<OrderDetailsDTO>
            {
                Result = orders.Result.Select(o => _mapper.Map<OrderDetailsDTO>(o)).ToList(),
                TotalCount = orders.TotalCount,
                PageIndex = parameters.PageIndex,
                PageSize = parameters.PageSize
            };
        }
    }
}
