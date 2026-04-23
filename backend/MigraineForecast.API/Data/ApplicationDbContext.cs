using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;
using System.Collections.Generic;

namespace MigraineForecast.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }

        // Tables
        public DbSet<Forecast> Forecasts { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<HealthCondition> HealthConditions { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<UserHealthCondition> UserHealthConditions { get; set; }
        public DbSet<UserSymptomSelection> UserSymptomSelections { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hardkodovani Admin korisnik
            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "admin123", // U realnom sistemu ovde ide Hash!
                Role = "Admin"
            });
        }


    }
}
