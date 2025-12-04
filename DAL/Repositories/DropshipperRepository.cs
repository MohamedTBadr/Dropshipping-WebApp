using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DAL.Repositories
{
    public class DropshipperRepository(DropShoppingDbContext dbContext) : IDropshipperRepository
    {
        public async Task<IEnumerable<Dropshipper>> GetAllDropshippersAsync()
        {
            return await dbContext.Dropshippers
                .Include(d => d.User) // ✅ load related User
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dropshipper> GetDropshipperByIdAsync(string userId)
        {
            return (await dbContext.Dropshippers.Include(d => d.User) 
         .AsNoTracking().Include(x=>x.Wallet).ThenInclude(x=>x.WalletTransactions)
         .FirstOrDefaultAsync(x => x.UserId == userId))!;

        }

        public async Task<Wallet> GetDropshipperWalletById(string userId)
        {
            var wallet = await dbContext.Wallets
                .Include(x => x.WalletTransactions)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DropshipperId == userId);

            if (wallet == null)
                throw new Exception("Wallet not found for this dropshipper.");

            return wallet;
        }

        public async Task CreateDropshipperAsync(Dropshipper dropshipper ,string UserId)
        {
            var dropshipper1 = new Dropshipper
            {
                UserId = UserId,
                Wallet = new Wallet { Balance = 0 }
            };
            await dbContext.Dropshippers.AddAsync(dropshipper1);
        }

        public async Task UpdateDropshipperAsync(Dropshipper dropshipper)
        {
            dbContext.Dropshippers.Update(dropshipper);
            await Task.CompletedTask;
        }

        public async Task<bool> DeleteDropshipperAsync(string userId)
            {
                var dropshipper = await dbContext.Dropshippers.FindAsync(userId);
                if (dropshipper == null)
                    return false;
    
                dbContext.Dropshippers.Remove(dropshipper);
                return true;
        }

   
    }
}
