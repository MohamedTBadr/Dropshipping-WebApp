using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL.DTOs;
using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IDropshipperRepository
    {
        Task<PaginatedResult<Dropshipper>> GetAllDropshippersAsync(int page);
        Task<Dropshipper> GetDropshipperByIdAsync(string userId);
        Task<Wallet> GetDropshipperWalletById(string userId);
        Task CreateDropshipperAsync(Dropshipper dropshipper , string UserId);

        Task UpdateDropshipperAsync(Dropshipper dropshipper);

        Task<bool> DeleteDropshipperAsync(string userId);

    }
}
