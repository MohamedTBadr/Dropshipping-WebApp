using BAL.DTOs.LoginDTO;
using BAL.DTOs.RegistrationDTOs;
using DAL;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PAL.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly DropShoppingDbContext _context;

        public AuthController(UserManager<User> userManager, IConfiguration config, DropShoppingDbContext context)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register(RegisterDto dto)
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

            // Create Dropshipper row if needed
            var User = await _userManager.FindByEmailAsync(user.Email);

            var dropshipper = new Dropshipper
            {
                UserId = User.Id,
                User = User,
                Wallet = new Wallet()
            };

            _context.Dropshippers.Add(dropshipper);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Account created successfully!" });
        }

        // ---------------------- LOGIN ----------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _userManager.Users
                .Include(u => u.Dropshipper)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            bool correctPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!correctPassword)
                return Unauthorized("Invalid email or password");

            string token = GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                UserId= user.Id,
                UserType = user.Dropshipper != null ? "DropShipper" : "Admin"
            });
        }

        // ------------------- JWT GENERATION ----------------------
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
