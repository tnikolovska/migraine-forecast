using System.ComponentModel.DataAnnotations;

namespace MigraineForecast.API.Models
{
    public class UserHealthCondition
    {
        public long Id { get; set; }
        [Required]
        public string UserId { get; set; }
 
        [Required]
        public int HealthConditionId { get; set; }
        
    }
}
