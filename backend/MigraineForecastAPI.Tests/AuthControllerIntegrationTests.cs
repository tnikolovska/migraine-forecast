using Microsoft.AspNetCore.Mvc.Testing;
using MigraineForecast.API.DTOs;
using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MigraineForecastAPI.Tests.Integration
{
    [Collection("Database collection")]
    public class AuthControllerIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(PostgreSqlFixture fixture)
        {
            _factory = new TestApiFactory(fixture.ConnectionString);
            _client = _factory.CreateClient();
        }

        /*private HttpClient CreateClient()
        {
            return _factory.CreateClient();
        }*/

        [Fact]
        public async Task Register_ReturnsToken()
        {
            

            var dto = new RegisterDto
            {
                Username = "testuser1",
                Password = "Test123!"
                
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result!.Token));
        }

        [Fact]
        public async Task Login_ReturnsToken_AndRole()
        {
            //var client = CreateClient();

            // First register user
            var registerDto = new RegisterDto
            {
                Username = "testuser2",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Then login
            var loginDto = new LoginDto
            {
                Username = "testuser2",
                Password = "Test123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result!.Token));
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
           // var client = CreateClient();

            var loginDto = new LoginDto
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Helpers for deserialization (match your API response shape)
        private class AuthResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}