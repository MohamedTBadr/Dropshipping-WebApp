using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.DTOs.WalletDTOs
{
    public class WalletDTO
    {
        public int WalletId { get; set; }

        public decimal Balance { get; set; }

        // Foreign key to Dropshipper
        public string DropshipperId { get; set; }

        public ICollection<WalletTransactionDTO> WalletTransactionDTO { get; set; }
    }



    public class WalletCreateDTO
    {
       

        public decimal Balance { get; set; }

        // Foreign key to Dropshipper
        public string DropshipperId { get; set; }

        public WalletCreateTransactionDTO WalletTransactionDTO { get; set; }
    }
}
