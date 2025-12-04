using DAL.Models;

namespace BAL.DTOs.DropshipperDTOs
{
    public class DropshipperDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Password { get; set; }  // PK and FK to ApplicationUser

        public Address Address { get; set; }

        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
    }
}
