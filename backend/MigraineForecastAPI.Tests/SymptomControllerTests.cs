using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using Xunit;

[Collection("Database collection")]
public class SymptomControllerTests
{
    private readonly HttpClient _client;

    public SymptomControllerTests(PostgreSqlFixture fixture)
    {
        var factory = new TestApiFactory(fixture.ConnectionString);
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/symptom");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}