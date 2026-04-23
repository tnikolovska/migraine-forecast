using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecast.API.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Globalization;
using System.Globalization;
using System.Text.Json;




namespace MigraineForecast.API.Services
{
    public class ForecastService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public ForecastService(ApplicationDbContext context, IConfiguration configuration,HttpClient httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<ForecastResultDto> GetForecastAsync(string userId, bool isAuthenticated)
        {
            var result = new ForecastResultDto();

            if (!isAuthenticated)
            {
                result.Success = false;
                result.Message = "User not authenticated";
                return result;
                //userId = "guest-user";
            }

            var hasCondition = await _context.UserHealthConditions
                .AnyAsync(u => u.UserId == userId);

            if (!hasCondition)
            {
                result.Success = false;
                result.Message = "User has no health condition";
                return result;
            }

            var apiUrl = _configuration["WeatherApi:BaseUrl"];
            var stopwatch = new Stopwatch();
            int retryCount = 0;
            const int maxRetries = 1;

            List<Forecast> list = new();

            while (retryCount <= maxRetries)
            {
                stopwatch.Restart();

                try
                {
                    _httpClient.Timeout = TimeSpan.FromSeconds(5);

                    var response = await _httpClient.GetAsync(apiUrl);
                    stopwatch.Stop();

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                       //samo za test treba da se brise     
                        /*var json = @"[
                            {
                                ""ID"": ""1"",
                                ""Name"": ""Migraine Index"",
                                ""LocalDateTime"": ""2026-04-06T16:00:00"",
                                ""Value"": 8.0,
                                ""Category"": ""At Risk"",
                                ""CategoryValue"": 1,
                                ""MobileLink"": ""http://test.com"",
                                ""Link"": ""http://test.com""
                                                }
                        ]";*/
                        //list = ParseJSON(json);



                        list = ParseJSON(json);

                        if (list.Any())
                        {
                            _context.Forecasts.AddRange(list);
                            await _context.SaveChangesAsync();
                        }

                        result.Success = true;
                        result.Data = list.Select(f => new ForecastDto
                        {
                            IdForecast = f.IdForecast,
                            Name = f.Name,
                            Date = f.Date,
                            Value = f.Value,
                            Category = f.Category,
                            CategoryValue = f.CategoryValue,
                            MobileLink = f.MobileLink,
                            Link = f.Link

                        }).ToList();

                        if (stopwatch.ElapsedMilliseconds > 2000)
                        {
                            result.Message = $"Forecast loaded slowly ({stopwatch.ElapsedMilliseconds}ms)";
                        }

                        return result;
                    }
                    else
                    {
                        result.Message = "Forecast service returned an error";
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = $"Greška: {ex.Message}";
                    return result;
                }


               /* catch (TaskCanceledException)
                {
                    stopwatch.Stop();
                    result.Message = "Forecast request timed out";
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    result.Message = $"Unexpected error: {ex.Message}";
                }*/

                retryCount++;

                if (retryCount > maxRetries)
                    break;

                await Task.Delay(500);
            }

            result.Success = false;
            result.Data = new List<ForecastDto>();

            return result;
        }

        private List<Forecast> ParseJSON(string forecastSearchResults)
        {
            List<Forecast> forecastList = new List<Forecast>();

            forecastSearchResults = "{ \"DailyIndexValues\": " + forecastSearchResults + " }";
            var rootObject = JObject.Parse(forecastSearchResults);
            var results = rootObject["DailyIndexValues"] as JArray;

            foreach (var result in results)
            {
                var rawDate = result["LocalDateTime"]?.ToString();
                DateTime parsedDate = string.IsNullOrEmpty(rawDate)
                    ? DateTime.UtcNow
                    : DateTime.Parse(rawDate);
                var forecast = new Forecast
                {
                    IdForecast = result["ID"]?.ToObject<string>() ?? "0",
                    Name = result["Name"]?.ToObject<string>() ?? "",
                    Date = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc),
                    Value = result["Value"]?.ToObject<decimal>() ?? 0,
                    Category = result["Category"]?.ToObject<string>() ?? "",
                    CategoryValue = result["CategoryValue"]?.ToObject<int>() ?? 0,
                    MobileLink = result["MobileLink"]?.ToObject<string>() ?? "",
                    Link = result["Link"]?.ToObject<string>() ?? ""
                };

                forecastList.Add(forecast);
            }

            return forecastList;
        }

        public async Task<List<ForecastDto>> GetAllForecastsAsync()
        {
            var forecasts = await _context.Forecasts.ToListAsync();

            return forecasts.Select(f => new ForecastDto
            {
                IdForecast = f.IdForecast,
                Name = f.Name,
                Date = f.Date,
                Value = f.Value,
                Category = f.Category,
                CategoryValue = f.CategoryValue,
                MobileLink = f.MobileLink,
                Link = f.Link
            }).ToList();
        }
    }
}