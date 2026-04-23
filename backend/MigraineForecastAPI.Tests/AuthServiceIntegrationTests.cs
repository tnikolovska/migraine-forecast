using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;
using MigraineForecastAPI.Tests.Fixtures;
using Xunit;

namespace MigraineForecastAPI.Tests.Integration
{
    public class AuthServiceIntegrationTests : IClassFixture<PostgreSqlFixture>
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;
        private readonly PostgreSqlFixture _fixture;

        public AuthServiceIntegrationTests(PostgreSqlFixture fixture)
        {
            _fixture = fixture;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

            _context = new ApplicationDbContext(options);
           

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "THIS_IS_A_SUPER_SECRET_TEST_KEY_123456789" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiresInMinutes", "60" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _passwordService = new PasswordService();

            _authService = new AuthService(_context, _passwordService, config);
        }

        // ---------------- REGISTER ----------------

        [Fact]
        public async Task RegisterAsync_CreatesUser_AndReturnsToken()
        {
            var dto = new RegisterDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var token = await _authService.RegisterAsync(dto);

            Assert.NotNull(token);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == "testuser");

            Assert.NotNull(user);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task RegisterAsync_Throws_WhenUserExists()
        {
            _context.Users.Add(new ApplicationUser
            {
                Username = "existing",
                PasswordHash = "hash",
                Role = "User"
            });
            await _context.SaveChangesAsync();

            var dto = new RegisterDto
            {
                Username = "existing",
                Password = "Password123!"
            };

            await Assert.ThrowsAsync<Exception>(() =>
                _authService.RegisterAsync(dto));
        }

        // ---------------- LOGIN ----------------

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsValid()
        {
            var user = new ApplicationUser
            {
                Username = "test",
                PasswordHash = _passwordService.HashPassword("Password123!"),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                Username = "test",
                Password = "Password123!"
            };

            var result = await _authService.LoginAsync(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenUserNotFound()
        {
            var dto = new LoginDto
            {
                Username = "ghost",
                Password = "Password123!"
            };

            var result = await _authService.LoginAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenPasswordWrong()
        {
            var user = new ApplicationUser
            {
                Username = "test",
                PasswordHash = _passwordService.HashPassword("CorrectPassword"),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                Username = "test",
                Password = "WrongPassword"
            };

            var result = await _authService.LoginAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_Works_ForAdmin_HardcodedCase()
        {
            var admin = new ApplicationUser
            {
                Username = "admin",
                PasswordHash = "admin123",
                Role = "Admin"
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                Username = "admin",
                Password = "admin123"
            };

            var result = await _authService.LoginAsync(dto);

            Assert.NotNull(result);
        }
    }
}