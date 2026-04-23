namespace MigraineForecast.API.Models
{
    public class Symptom
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long HealthConditionId { get; set; }

        public MigraineType Type { get; set; }
    }
}
