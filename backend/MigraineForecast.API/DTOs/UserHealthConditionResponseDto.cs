namespace MigraineForecast.API.DTOs
{
    public class UserHealthConditionResponseDto
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int HealthConditionId { get; set; }
    }
}
