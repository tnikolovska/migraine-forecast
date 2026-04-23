using System.ComponentModel.DataAnnotations;

namespace MigraineForecast.API.Models
{
    public class ApplicationUser
    {
        public long Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";
    }
}
