using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.OrderDTOs
{
    public class OrderParameters
    {
        public string? Status { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
