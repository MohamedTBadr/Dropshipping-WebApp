using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Dropshipper
    {
        [Key]
        public string UserId { get; set; }  // PK and FK to ApplicationUser 

        // Navigation property to User
        public virtual User User { get; set; }

        // Navigation property to Wallet (one-to-one)
        public virtual Wallet Wallet { get; set; }
    }




}
