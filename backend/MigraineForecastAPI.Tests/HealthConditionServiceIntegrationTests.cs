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
    public class HealthConditionServiceIntegrationTests
    {
        private readonly PostgreSqlFixture _fixture;

        public HealthConditionServiceIntegrationTests(PostgreSqlFixture fixture)
        {
            _fixture = fixture;
        }

        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_fixture.ConnectionString)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.Migrate(); // ensure schema

            return context;
        }

        private async Task SeedData(ApplicationDbContext context)
        {
            var condition = new HealthCondition
            {
                Name = "Migraine",
                Description = "Headache",
                Symptoms = new List<Symptom>
                {
                    new Symptom
                    {
                        Name = "Nausea",
                        Description = "Feeling sick",
                        Type = MigraineType.BeforeHeadache
                    }
                }
            };

            context.HealthConditions.Add(condition);
            await context.SaveChangesAsync();
        }

        // ✅ GET ALL
        [Fact]
        public async Task GetAllAsync_ReturnsData_FromDatabase()
        {
            using var context = CreateContext();
            await SeedData(context);

            var service = new HealthConditionService(context);

            var result = await service.GetAllAsync();

            Assert.NotEmpty(result);
            Assert.Equal("Migraine", result[0].Name);
            Assert.Single(result[0].Symptoms);
        }

        // ✅ GET BY ID
        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectEntity()
        {
            using var context = CreateContext();
            await SeedData(context);

            var service = new HealthConditionService(context);

            var entity = await context.HealthConditions.FirstAsync();

            var result = await service.GetByIdAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Name, result.Name);
        }

        // ❌ GET BY ID NOT FOUND
        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            using var context = CreateContext();
            var service = new HealthConditionService(context);

            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }

        // ✅ CREATE
        [Fact]
        public async Task CreateAsync_PersistsToDatabase()
        {
            using var context = CreateContext();
            var service = new HealthConditionService(context);

            var dto = new HealthConditionDto
            {
                Name = "Migraine",
                Description = "Migraine"
            };

            var result = await service.CreateAsync(dto);

            var dbEntity = await context.HealthConditions.FirstOrDefaultAsync();

            Assert.NotNull(dbEntity);
            Assert.Equal("Migraine", dbEntity.Name);
        }

        // ✅ DELETE
        /*[Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            using var context = CreateContext();
            await SeedData(context);

            var service = new HealthConditionService(context);
            var entity = await context.HealthConditions.FirstAsync();

            var result = await service.DeleteAsync(entity.Id);

            Assert.True(result);
            Assert.Empty(context.HealthConditions);
        }*/

        [Fact]
        public async Task DeleteAsync_RemovesEntity()
        {
            using var context = CreateContext();
            await SeedData(context);

            var service = new HealthConditionService(context);
            var entity = await context.HealthConditions.FirstAsync();

            var result = await service.DeleteAsync(entity.Id);

            Assert.True(result);

            // Reload from DB
            var deleted = await context.HealthConditions.FindAsync(entity.Id);

            Assert.Null(deleted);
        }

        // ❌ DELETE NOT FOUND
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = CreateContext();
            var service = new HealthConditionService(context);

            var result = await service.DeleteAsync(999);

            Assert.False(result);
        }

        // ✅ UPDATE
        [Fact]
        public async Task UpdateAsync_UpdatesEntity()
        {
            using var context = CreateContext();
            await SeedData(context);

            var service = new HealthConditionService(context);
            var entity = await context.HealthConditions.FirstAsync();

            entity.Name = "Updated";

            var result = await service.UpdateAsync(entity.Id, entity);

            Assert.True(result);

            var updated = await context.HealthConditions.FindAsync(entity.Id);
            Assert.Equal("Updated", updated.Name);
        }
    }
}