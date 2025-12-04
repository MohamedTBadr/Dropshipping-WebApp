using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.DropshipperDTOs
{
    public class DropshipperUpdate
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string? Password { get; set; }  // PK and FK to ApplicationUser

        public Address Address { get; set; }

        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }

    }
}
