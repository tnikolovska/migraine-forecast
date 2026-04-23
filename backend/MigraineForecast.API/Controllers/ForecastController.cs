using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraineForecast.API.Services;
using System.Security.Claims;

namespace MigraineForecast.API.Controllers
{
    [Route("api/[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly ForecastService _service;

        public ForecastController(ForecastService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,User")]
      
        [HttpGet]
        public async Task<IActionResult> GetForecast()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            var result = await _service.GetForecastAsync(userId,isAuthenticated);
           

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);

           
        }
        [Authorize(Roles = "Admin,User")]
      
        [HttpGet("all")]
        public async Task<IActionResult> GetAllForecasts()
        {
            var result = await _service.GetAllForecastsAsync();
            return Ok(result);
        }
    }
}
