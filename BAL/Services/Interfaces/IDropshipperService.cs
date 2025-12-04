using BAL.DTOs.DropshipperDTOs;

namespace BAL.Services.Interfaces
{
    public interface IDropshipperService
    {
        Task<IEnumerable<DropshipperDetails>> GetAllDropshippersAsync();
        Task<DropshipperDetails?> GetDropshipperByIdAsync(string userId);
        Task<DTOs.DropshipperDTOs.Wallet> GetDropshipperWalletByIdAsync(string userId);

        Task CreateDropshipperAsync(DropshipperDto dropshipperDto);
        Task<DropshipperUpdate?> UpdateDropshipperAsync(DropshipperUpdate dropshipperDto);
        Task DeleteDropshipperAsync(string userId);
    }
}
