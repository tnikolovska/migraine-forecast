using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Services;

namespace MigraineForecast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _context;

        public AuthController(AuthService authService,ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            /*var token = await _authService.LoginAsync(dto);

            if (token == null)
                return Unauthorized();

            return Ok(new { token });*/

            // 1. Authenticate and get the token string
            var token = await _authService.LoginAsync(dto);

            if (token == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // 2. Fetch the user to get their role
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            // 3. Send back BOTH token and role
            return Ok(new
            {
                token = token,
                role = user?.Role ?? "User"
            });
        }
    }
}
