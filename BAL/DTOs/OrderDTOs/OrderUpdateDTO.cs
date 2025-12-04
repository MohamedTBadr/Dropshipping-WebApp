using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.OrderDTOs
{
    public class OrderUpdateDTO
    {
        public Guid Id { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal OrderDiscount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateOnly? ShippedDate { get; set; }
    }
}
