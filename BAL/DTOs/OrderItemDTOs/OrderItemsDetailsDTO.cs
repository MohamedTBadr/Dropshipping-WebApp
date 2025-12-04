using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.OrderItemDTOs
{
    public class OrderItemsDetailsDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }        // filled from Product.Price
        public decimal OrderItemDiscount { get; set; } // calculated by server if applied
    }
}
