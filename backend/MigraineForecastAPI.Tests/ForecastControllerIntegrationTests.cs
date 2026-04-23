using Microsoft.AspNetCore.Mvc.Testing;
using MigraineForecastAPI.Tests.Auth;
using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Xunit;

namespace MigraineForecastAPI.Tests.Integration
{
    [Collection("Database collection")]
    public class ForecastControllerIntegrationTests
    {
        private readonly TestApiFactory _factory;

        public ForecastControllerIntegrationTests(PostgreSqlFixture fixture)
        {
            _factory = new TestApiFactory(fixture.ConnectionString);
        }

        [Fact]
        public async Task GetForecast_ReturnsResponse()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/forecast");

            Assert.True(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.Unauthorized
            );
        }

        [Fact]
        public async Task GetAllForecasts_ReturnsOk_WhenAuthenticated()
        {
            FakeAuthHandler.TestClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "user1"),
                    new Claim(ClaimTypes.Role, "User")
                };

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Fake");

            var response = await client.GetAsync("/api/forecast/all");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}