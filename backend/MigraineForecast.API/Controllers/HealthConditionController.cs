using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;
using MigraineForecast.API.Services;

namespace MigraineForecast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthConditionController : ControllerBase
    {
        private readonly HealthConditionService _service;

        public HealthConditionController(HealthConditionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HealthConditionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);

            if (!success)
                return NotFound();

            return Ok("Deleted");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCondition(long id, HealthCondition condition)
        {
            var success = await _service.UpdateAsync(id, condition);
            if (!success) return BadRequest("Unable to update condition.");

            return Ok(condition);
        }
    }
}
