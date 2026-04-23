using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Services;
using System.Security.Claims;

namespace MigraineForecast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserHealthConditionController : ControllerBase
    {
        private readonly UserHealthConditionService _service;

        public UserHealthConditionController(UserHealthConditionService service)
        {
            _service = service;
        }

        // ✅ ADD condition to logged user
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create(UserHealthConditionDto dto)
        {
            Console.WriteLine($"USER: {User?.Identity?.Name}");
            Console.WriteLine($"DTO HC ID: {dto.HealthConditionId}");



            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"USER ID CLAIM: {userId}");
            var result = await _service.CreateAsync(userId, dto);

            if (result == null)
                return Conflict(new { message = "Condition already assigned" });

            return Ok(result);
        }

        // ✅ GET my conditions
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetMyConditions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(await _service.GetByUserAsync(userId));
        }

        // ✅ DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound();

            return Ok("Deleted");
        }
    }
}
