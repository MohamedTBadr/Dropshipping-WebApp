using BAL.DTOs.CustomerDTOs;
using BAL.DTOs.OrderItemDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.OrderDTOs
{
    public class OrderCreateDTO
    {
        public string DropshipperId { get; set; }
        public List<OrderItemCreateDTO> Items { get; set; } = new();

        public CustomerDetailsDTO Customer { get; set; }

        public DateOnly? ShippedDate { get; set; }
    }
}
