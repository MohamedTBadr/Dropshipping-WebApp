using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.WalletDTOs
{
    public class WalletTransactionDTO
    {
        public int TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        // Foreign key to Wallet
        public int WalletId { get; set; }

        //// Navigation property to Wallet
        //public WalletDTO Wallet { get; set; }
    }

    public class WalletCreateTransactionDTO
    {
      

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        // Foreign key to Wallet
        public int WalletId { get; set; }

     
    }
}
