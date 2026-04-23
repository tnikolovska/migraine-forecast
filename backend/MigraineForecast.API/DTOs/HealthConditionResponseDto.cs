namespace MigraineForecast.API.DTOs
{
    public class HealthConditionResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<SymptomDto> Symptoms { get; set; } = new();
    }
}
