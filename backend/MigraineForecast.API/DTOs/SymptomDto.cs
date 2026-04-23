using MigraineForecast.API.Models;

namespace MigraineForecast.API.DTOs
{
    public class SymptomDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long HealthConditionId { get; set; }
        public string Type { get; set; }
    }
}
