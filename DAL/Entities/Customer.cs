using System;

namespace DAL.Models
{
    public class Customer:BaseEntity
    {
       
        public string  Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public string DropshipperId { get; set; }
        public User Dropshipper { get; set; }



        public ICollection<Order> Orders { get; set; }
    }
}
