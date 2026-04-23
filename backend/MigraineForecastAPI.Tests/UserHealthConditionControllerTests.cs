using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecastAPI.Tests.Auth;
using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Xunit;

namespace MigraineForecastAPI.Tests.Integration
{
    [Collection("Database collection")]
    public class UserHealthConditionControllerTests
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;

        public UserHealthConditionControllerTests(PostgreSqlFixture fixture)
        {
            _factory = new TestApiFactory(fixture.ConnectionString);
            _client = _factory.CreateClient();
        }

        // 🔑 helper to create authenticated client
        private HttpClient CreateAuthenticatedClient(string userId, string role)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Fake")
                        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
                            "Fake", options => { });

                    services.PostConfigure<AuthenticationOptions>(options =>
                    {
                        options.DefaultAuthenticateScheme = "Fake";
                        options.DefaultChallengeScheme = "Fake";
                    });


                });
            }).CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Fake");

            FakeAuthHandler.TestClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, "Test User"),
        new Claim(ClaimTypes.Role, role)
    };

            return client;
        }

        // ✅ CREATE
        [Fact]
        public async Task Create_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            var client = CreateAuthenticatedClient(userId, "User");

            // seed HealthCondition
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.HealthConditions.Add(new HealthCondition
                {
                    Name = "Migraine",
                    Description = "Headache"
                });

                await context.SaveChangesAsync();
            }

            var dto = new UserHealthConditionDto { HealthConditionId = 1 };

            var response = await client.PostAsJsonAsync("/api/userhealthcondition", dto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ✅ GET MY CONDITIONS
        [Fact]
        public async Task GetMyConditions_ReturnsUserData()
        {
            var userId = Guid.NewGuid().ToString();
            var client = CreateAuthenticatedClient(userId, "User");

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var condition = new HealthCondition
                {
                    Name = "Migraine",
                    Description = "Headache"
                };

                context.HealthConditions.Add(condition);
                await context.SaveChangesAsync();

                context.UserHealthConditions.Add(new UserHealthCondition
                {
                    UserId = userId,
                    HealthConditionId = (int)condition.Id
                });

                await context.SaveChangesAsync();
            }

            var response = await client.GetAsync("/api/userhealthcondition");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<List<UserHealthConditionResponseDto>>();

            Assert.Single(result);
            Assert.Equal(userId, result[0].UserId);
        }

        // ✅ DELETE (Admin only)
        [Fact]
        public async Task Delete_AsAdmin_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            var client = CreateAuthenticatedClient(userId, "Admin");

            long id;

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var condition = new HealthCondition
                {
                    Name = "Migraine",
                    Description = "Headache"
                };

                context.HealthConditions.Add(condition);
                await context.SaveChangesAsync();

                var entity = new UserHealthCondition
                {
                    UserId = userId,
                    HealthConditionId = (int)condition.Id
                };

                context.UserHealthConditions.Add(entity);
                await context.SaveChangesAsync();

                id = entity.Id;
            }

            var response = await client.DeleteAsync($"/api/userhealthcondition/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ❌ DELETE forbidden for User role
        [Fact]
        public async Task Delete_AsUser_ReturnsForbidden()
        {
            var userId = Guid.NewGuid().ToString();
            var client = CreateAuthenticatedClient(userId, "User");

            var response = await client.DeleteAsync("/api/userhealthcondition/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}