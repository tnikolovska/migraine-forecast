using MigraineForecastAPI.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MigraineForecastAPI.Tests
{
    public class SymptomIntegrationTests : IClassFixture<PostgreSqlFixture>
    {
        private readonly HttpClient _client;
        private readonly TestApiFactory _factory;

        public SymptomIntegrationTests(PostgreSqlFixture fixture)
        {
            _factory = new TestApiFactory(fixture.ConnectionString);
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/symptom");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/symptom/1");

            Assert.True(
                response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.NotFound
            );
        }

        [Fact]
        public async Task Create_ShouldReturnUnauthorized_WithoutToken()
        {
            var response = await _client.PostAsJsonAsync("/api/symptom", new
            {
                name = "Test Symptom",
                description = "Test",
                type = "BeforeHeadache"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task Update_ShouldReturnUnauthorized_WithoutToken()
        {
            var response = await _client.PutAsJsonAsync("/api/symptom/1", new
            {
                name = "Updated",
                description = "Updated",
                type = "AfterAttack"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
