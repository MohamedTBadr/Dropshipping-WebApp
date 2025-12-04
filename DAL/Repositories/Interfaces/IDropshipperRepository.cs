using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IDropshipperRepository
    {
        Task<IEnumerable<Dropshipper>> GetAllDropshippersAsync();
        Task<Dropshipper> GetDropshipperByIdAsync(string userId);
        Task<Wallet> GetDropshipperWalletById(string userId);
        Task CreateDropshipperAsync(Dropshipper dropshipper , string UserId);

        Task UpdateDropshipperAsync(Dropshipper dropshipper);

        Task<bool> DeleteDropshipperAsync(string userId);

    }
}
