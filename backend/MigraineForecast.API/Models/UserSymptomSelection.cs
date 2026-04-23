namespace MigraineForecast.API.Models
{
    public class UserSymptomSelection
    {
        public long Id { get; set; }
        public int UserHealthConditionId { get; set; }
        public ICollection<Symptom> MigraineSymptoms { get; set; } = new List<Symptom>();
    }
}
