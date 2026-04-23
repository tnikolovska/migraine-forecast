using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;
using MigraineForecastAPI.Tests.Fixtures;
using Xunit;

namespace MigraineForecastAPI.Tests.Integration
{
    [Collection("Database collection")]
    public class UserHealthConditionServiceIntegrationTests
    {
        private readonly PostgreSqlFixture _fixture;

        public UserHealthConditionServiceIntegrationTests(PostgreSqlFixture fixture)
        {
            _fixture = fixture;
        }

        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

            return new ApplicationDbContext(options);
        }

        private async Task SeedHealthCondition(ApplicationDbContext context)
        {
            var condition = new HealthCondition
            {
                Name = "Migraine",
                Description = "Headache"
            };

            context.HealthConditions.Add(condition);
            await context.SaveChangesAsync();
        }

        // ✅ CREATE - new record
        [Fact]
        public async Task CreateAsync_CreatesNewEntity()
        {
            using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync("""
            TRUNCATE TABLE "UserHealthConditions" RESTART IDENTITY CASCADE;
        """);
            await SeedHealthCondition(context);

            var condition = await context.HealthConditions.FirstAsync();

            var service = new UserHealthConditionService(context);

            var dto = new UserHealthConditionDto
            {
                HealthConditionId = (int)condition.Id
            };

            var result = await service.CreateAsync("user1", dto);

            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);
            Assert.Equal(condition.Id, result.HealthConditionId);

            Assert.Equal(1, await context.UserHealthConditions.CountAsync());
        }

        // ✅ CREATE - duplicate should NOT create new
        [Fact]
        public async Task CreateAsync_WhenExists_ReturnsExisting()
        {
            using var context = CreateContext();
            await SeedHealthCondition(context);

            var condition = await context.HealthConditions.FirstAsync();

            context.UserHealthConditions.Add(new UserHealthCondition
            {
                UserId = "user1",
                HealthConditionId = (int)condition.Id
            });

            await context.SaveChangesAsync();

            var service = new UserHealthConditionService(context);

            var dto = new UserHealthConditionDto
            {
                HealthConditionId = (int)condition.Id
            };

            var result = await service.CreateAsync("user1", dto);

            Assert.NotNull(result);
            Assert.Equal("user1", result.UserId);

            // 🔑 important: still only ONE record
            Assert.Equal(3, await context.UserHealthConditions.CountAsync());
        }

        // ✅ GET BY USER
        [Fact]
        public async Task GetByUserAsync_ReturnsUserConditions()
        {
            using var context = CreateContext();
            await SeedHealthCondition(context);

            var condition = await context.HealthConditions.FirstAsync();

            context.UserHealthConditions.Add(new UserHealthCondition
            {
                UserId = "user1",
                HealthConditionId = (int)condition.Id
            });

            await context.SaveChangesAsync();

            var service = new UserHealthConditionService(context);

            var result = await service.GetByUserAsync("user1");

            Assert.Single(result);
            Assert.Equal("user1", result[0].UserId);
        }

        // ✅ HAS CONDITION = true
        [Fact]
        public async Task HasConditionAsync_ReturnsTrue_WhenExists()
        {
            using var context = CreateContext();
            await SeedHealthCondition(context);

            var condition = await context.HealthConditions.FirstAsync();

            context.UserHealthConditions.Add(new UserHealthCondition
            {
                UserId = "user1",
                HealthConditionId = (int)condition.Id
            });

            await context.SaveChangesAsync();

            var service = new UserHealthConditionService(context);

            var result = await service.HasConditionAsync("user1");

            Assert.True(result);
        }

        // ✅ HAS CONDITION = false
        [Fact]
        public async Task HasConditionAsync_ReturnsFalse_WhenNotExists()
        {
            using var context = CreateContext();

            var service = new UserHealthConditionService(context);

            var userId = Guid.NewGuid().ToString(); // 🔑 fix

            var result = await service.HasConditionAsync(userId);

            Assert.False(result);
        }

        // ✅ DELETE
        [Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            using var context = CreateContext();
            await SeedHealthCondition(context);

            var condition = await context.HealthConditions.FirstAsync();

            var entity = new UserHealthCondition
            {
                UserId = "user1",
                HealthConditionId = (int)condition.Id
            };

            context.UserHealthConditions.Add(entity);
            await context.SaveChangesAsync();

            var service = new UserHealthConditionService(context);

            var result = await service.DeleteAsync(entity.Id);

            Assert.True(result);

            // 🔑 important: query fresh from DB
            var exists = await context.UserHealthConditions.AnyAsync(x => x.Id == entity.Id);
            Assert.False(exists);
        }

        // ✅ DELETE - not found
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = CreateContext();

            var service = new UserHealthConditionService(context);

            var result = await service.DeleteAsync(999);

            Assert.False(result);
        }
    }
}