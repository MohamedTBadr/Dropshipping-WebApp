using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        public decimal Balance { get; set; }

        // Foreign key to Dropshipper
        public string DropshipperId { get; set; }

        // Navigation property to Dropshipper
        public virtual Dropshipper Dropshipper { get; set; }

        // Navigation property to WalletTransactions (one-to-many)
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }




}
