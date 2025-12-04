using AutoMapper;
using BAL.DTOs.DropshipperDTOs;
using BAL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class DropshipperService(IDropshipperRepository dropshipperRepository,IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> _userManager) : IDropshipperService
    {

        public async Task CreateDropshipperAsync(DropshipperDto dropshipperDto)
        {
            var user = await _userManager.FindByEmailAsync(dropshipperDto.Email);

            if (user == null)
            {
                var newUser = new User
                {
                    Email = dropshipperDto.Email,
                    PhoneNumber = dropshipperDto.PhoneNumber,
                    UserName = dropshipperDto.UserName,
                    Address = dropshipperDto.Address,
                    IsActive = true,
                    CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
                };

                var result = await _userManager.CreateAsync(newUser, dropshipperDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"User creation failed: {errors}");
                }

                user = newUser; 
            }

            var mapped = mapper.Map<Dropshipper>(dropshipperDto);
            await dropshipperRepository.CreateDropshipperAsync(mapped, user.Id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDropshipperAsync(string userId)
        {
            await dropshipperRepository.DeleteDropshipperAsync(userId);
        }

        public async Task<IEnumerable<DropshipperDetails>> GetAllDropshippersAsync()
        {
            var dropshippers = await dropshipperRepository.GetAllDropshippersAsync();
            var mapped = mapper.Map<IEnumerable<DropshipperDetails>>(dropshippers);       
            return mapped;
        }


        public async Task<DropshipperDetails?> GetDropshipperByIdAsync(string userId)
        {
            var dropshipper = await dropshipperRepository.GetDropshipperByIdAsync(userId);
            if (dropshipper == null)
                return null;
            if (dropshipper.Wallet != null)
            {
                Console.WriteLine($"Transactions: {dropshipper.Wallet.WalletTransactions.Count}");
            }
            var mapped = mapper.Map<DropshipperDetails>(dropshipper);
            return mapped;
        }

        public async Task<DTOs.DropshipperDTOs.Wallet> GetDropshipperWalletByIdAsync(string userId)
        {
            var wallet = await dropshipperRepository.GetDropshipperWalletById(userId);
            if (wallet == null)
                return null;

            var mappedWallet = mapper.Map<DTOs.DropshipperDTOs.Wallet>(wallet);
            return mappedWallet;
        }

        public async Task<DropshipperUpdate?> UpdateDropshipperAsync(DropshipperUpdate dropshipperDto)
        {
            // Load existing dropshipper + user
            var existingDropshipper = await dropshipperRepository.GetDropshipperByIdAsync(dropshipperDto.Id);
            if (existingDropshipper == null)
                return null;

            // Update User fields (since DTO contains them)
            var user = existingDropshipper.User;
            if (user != null)
            {
                user.UserName = dropshipperDto.UserName;
                user.Email = dropshipperDto.Email;
                user.PhoneNumber = dropshipperDto.PhoneNumber;
                user.Address = dropshipperDto.Address;
                user.IsActive = dropshipperDto.IsActive;
                user.CreatedAt = dropshipperDto.CreatedAt;
            }

            // Optionally update dropshipper-specific fields here (if any)
            // e.g. existingDropshipper.Wallet.Balance = dropshipperDto.Balance;

            await dropshipperRepository.UpdateDropshipperAsync(existingDropshipper);
            await unitOfWork.SaveChangesAsync();

            return dropshipperDto;
        }

    }
}
