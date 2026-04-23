namespace MigraineForecast.API.DTOs
{
    public class ForecastResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<ForecastDto> Data { get; set; } = new();
    }
}
