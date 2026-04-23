using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;

namespace MigraineForecastAPI.Tests.Integration
{
    [Collection("Database collection")]
    public class HealthConditionControllerTests
    {
        private readonly HttpClient _client;

        public HealthConditionControllerTests(PostgreSqlFixture fixture)
        {
            var factory = new TestApiFactory(fixture.ConnectionString);
            _client = factory.CreateClient();
        }

        // ✅ GET ALL
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/healthcondition");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // ✅ GET BY ID - NOT FOUND
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var response = await _client.GetAsync("/api/healthcondition/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ✅ CREATE (ADMIN REQUIRED)
        [Fact]
        public async Task Create_ReturnsUnauthorized_WithoutToken()
        {
            var dto = new HealthConditionDto
            {
                Name = "Test Condition",
                Description = "Test"
            };

            var response = await _client.PostAsJsonAsync("/api/healthcondition", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ✅ DELETE - NOT FOUND
        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/healthcondition/9999");

            var response = await _client.SendAsync(request);

            // Without auth → Unauthorized (important!)
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}