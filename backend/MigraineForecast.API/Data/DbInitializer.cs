using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;

namespace MigraineForecast.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<PasswordService>();

            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var admin = new ApplicationUser
                {
                    Username = "admin",
                    PasswordHash = passwordService.HashPassword("admin123"),
                    Role = "Admin"
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}