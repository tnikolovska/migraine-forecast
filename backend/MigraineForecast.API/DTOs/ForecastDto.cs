using System.Text.Json.Serialization;

namespace MigraineForecast.API.DTOs
{
    public class ForecastDto
    {
        [JsonPropertyName("ID")]
        public string IdForecast { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Category { get; set; }
        public decimal CategoryValue { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }
}
