using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MigraineForecast.API.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace MigraineForecastAPI.Tests.Fixtures
{
    public class PostgreSqlFixture : IAsyncLifetime
    {
        /* private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
             /*.WithDatabase("testdb")
             .WithUsername("postgres")
             .WithPassword("postgres")
             .Build();*/
        /*.WithImage("postgres:16-alpine")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();*/

        private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:16-alpine")
          .WithImage("postgres:16-alpine")
          .WithDatabase("migraine_db")
          .WithUsername("postgres")
          .WithPassword("password")
          .Build();

        public string ConnectionString { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            await _db.StartAsync();
            ConnectionString = _db.GetConnectionString();





            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseNpgsql(_db.GetConnectionString())
                   .Options;

            using var context = new ApplicationDbContext(options);
            await context.Database.MigrateAsync(); // 🔥 IMPORTANT

        }

        public Task DisposeAsync() => _db.DisposeAsync().AsTask();
    }
}