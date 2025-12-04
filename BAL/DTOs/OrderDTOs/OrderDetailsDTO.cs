using BAL.DTOs.OrderItemDTOs;
using DAL.Models;
using System;
using System.Collections.Generic;

namespace BAL.DTOs.OrderDTOs
{
    public class OrderDetailsDTO
    {
        public Guid Id { get; set; }
        public DateOnly? ShippedDate { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal OrderDiscount { get; set; }

        // Customer info
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string DropshipperId { get; set; }
        public string DropshipperName { get; set; }

        public List<OrderItemsDetailsDTO> Items { get; set; } = new();
    }
}
