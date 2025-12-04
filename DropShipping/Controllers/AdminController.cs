using BAL.DTOs.RegistrationDTOs;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;


    public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = new User
            {
                UserName = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                IsActive = true,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Address = new Address()
                {
                    City = dto.Address_City,
                    Country = dto.Address_Country,
                    Street = dto.Address_Street
                }
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            //// Assign Admin role
            //if (!await _userManager.IsInRoleAsync(user, "Admin"))
            //{
            //    var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
            //    if (!roleResult.Succeeded)
            //        return BadRequest(roleResult.Errors);
            //}

            return Created();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpGet]
        public IActionResult GetAllAdmins()
        {
            var users = _userManager.Users
                        .Where(x => x.Dropshipper == null)
                        .ToList();

            var adminDtos = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.IsActive,
                user.CreatedAt,
                Address = new
                {
                    user.Address.Street,
                    user.Address.City,
                    user.Address.Country
                }
            });

            return Ok(adminDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            var adminDto = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.IsActive,
                user.CreatedAt,
                Address = new
                {
                    user.Address.Street,
                    user.Address.City,
                    user.Address.Country
                }
            };

            return Ok(adminDto);
        }


        //[HttpPatch]
        //public async Task<IActionResult> UpdateAdmin(string i)
        //{

        //}
    }


}
