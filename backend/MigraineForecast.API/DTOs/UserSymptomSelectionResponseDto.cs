namespace MigraineForecast.API.DTOs
{
    public class UserSymptomSelectionResponseDto
    {
        public long Id { get; set; }
        public int UserHealthConditionId { get; set; }

        public List<SymptomDto> Symptoms { get; set; } = new();
    }
}
