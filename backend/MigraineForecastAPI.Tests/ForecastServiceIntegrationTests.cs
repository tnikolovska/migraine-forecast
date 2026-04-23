using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;
using MigraineForecastAPI.Tests.Fixtures;
using System.Net;
using System.Text;
using Xunit;

[Collection("Database collection")]
public class ForecastServiceIntegrationTests
{
    private readonly PostgreSqlFixture _fixture;

    public ForecastServiceIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        return context;
    }

    private IConfiguration CreateConfig()
    {
        var dict = new Dictionary<string, string>
        {
            { "WeatherApi:BaseUrl", "http://fake-api.com" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict!)
            .Build();
    }

    [Fact]
    public async Task GetForecastAsync_ReturnsError_WhenNotAuthenticated()
    {
        var context = CreateContext();
        var config = CreateConfig();

        var httpClient = new HttpClient(new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)));

        var service = new ForecastService(context, config,httpClient);

        var result = await service.GetForecastAsync("user1", false);

        Assert.False(result.Success);
        Assert.Equal("User not authenticated", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_ReturnsError_WhenUserHasNoCondition()
    {
        var context = CreateContext();
        var config = CreateConfig();

        var httpClient = new HttpClient(new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)));

        var service = new ForecastService(context, config,httpClient);

        var result = await service.GetForecastAsync("user1", true);

        Assert.False(result.Success);
        Assert.Equal("User has no health condition", result.Message);
    }

    [Fact]
    public async Task GetForecastAsync_SavesForecast_WhenApiReturnsData()
    {

        var context = CreateContext();
        var config = CreateConfig();


        var condition = new HealthCondition {Name = "Migraine" ,Description=
            "Migraine"};
        context.HealthConditions.Add(condition);
        await context.SaveChangesAsync();

        // Seed condition
        context.UserHealthConditions.Add(new UserHealthCondition
        {
            UserId = "user1",
            HealthConditionId = (int)condition.Id
        });
        await context.SaveChangesAsync();

        var json = @"[
    {
        ""ID"": ""1"",
        ""Name"": ""Migraine Index"",
        ""LocalDateTime"": ""2026-04-06T16:00:00"",
        ""Value"": 8.0,
        ""Category"": ""At Risk"",
        ""CategoryValue"": 1,
        ""MobileLink"": ""http://test.com"",
        ""Link"": ""http://test.com""
    }]";

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var httpClient = new HttpClient(new FakeHttpMessageHandler(response));

        var service = new ForecastService(context, config,httpClient);

        var result = await service.GetForecastAsync("user1", true);

        Assert.True(result.Success);
        Assert.Single(result.Data);

        var count = await context.Forecasts.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetForecastAsync_ReturnsError_WhenApiFails()
    {
        var context = CreateContext();
        var config = CreateConfig();

        context.UserHealthConditions.Add(new UserHealthCondition
        {
            UserId = "user1",
            HealthConditionId = 1
        });
        await context.SaveChangesAsync();

        var httpClient = new HttpClient(new FakeHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        var service = new ForecastService(context, config,httpClient);

        var result = await service.GetForecastAsync("user1", true);

        Assert.False(result.Success);
        // Assert.Equal("Greška: This instance has already started", result.Message);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public async Task GetAllForecastsAsync_ReturnsData()
    {
        var context = CreateContext();

        context.Forecasts.Add(new Forecast
        {
            IdForecast = "1",
            Name = "Test",
            Date = DateTime.UtcNow,
            Value = 5,
            Category = "Low",
            CategoryValue = 1,
            Link = "http://test.com",
            MobileLink = "http://test.com"
        });

        await context.SaveChangesAsync();

        var config = CreateConfig();
        var httpClient = new HttpClient();

        var service = new ForecastService(context, config,httpClient);

        var result = await service.GetAllForecastsAsync();

        Assert.Single(result);
    }

}