using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.DropshipperDTOs
{
    public class Wallet
    {
        public decimal Balance { get; set; }

        public ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
