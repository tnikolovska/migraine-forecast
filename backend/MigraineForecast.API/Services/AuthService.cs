using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MigraineForecast.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, PasswordService passwordService, IConfiguration config)
        {
            _context = context;
            _passwordService = passwordService;
            _config = config;
        }

        // ✅ REGISTER
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(x => x.Username == dto.Username);

            if (exists)
                throw new Exception("User already exists");

            var user = new ApplicationUser
            {
                Username = dto.Username,
                PasswordHash = _passwordService.HashPassword(dto.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return GenerateToken(user);
         
        }

        // ✅ LOGIN
        public async Task<string?> LoginAsync(LoginDto dto)
        {

            // 1. Case-insensitive lookup
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (user == null) return null;

            // 2. Manual bypass for the hardcoded admin (since its hash is just "admin123")
            if (user.Username == "admin" && user.PasswordHash == "admin123")
            {
                if (dto.Password == "admin123") return GenerateToken(user);
            }

            // 3. BCrypt verification for everyone else (like 'test')
            if (!_passwordService.Verify(dto.Password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        // 🔑 JWT
        private string GenerateToken(ApplicationUser user)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwt["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
