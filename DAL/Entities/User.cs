using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class User:IdentityUser
    {

        public Address Address {  get; set; }
        // Navigation property to Dropshipper (nullable)
        public virtual Dropshipper Dropshipper { get; set; }
        public bool IsActive { get; set; }
        public DateOnly CreatedAt { get; set; }
    }




}
