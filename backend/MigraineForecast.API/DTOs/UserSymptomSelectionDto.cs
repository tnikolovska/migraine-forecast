namespace MigraineForecast.API.DTOs
{
    public class UserSymptomSelectionDto
    {
        public int UserHealthConditionId { get; set; }

        // 👇 клучно
        public List<long> SymptomIds { get; set; } = new();
    }
}
