using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class WalletTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        // Foreign key to Wallet
        public int WalletId { get; set; }

        // Navigation property to Wallet
        public virtual Wallet Wallet { get; set; }
    }




}
