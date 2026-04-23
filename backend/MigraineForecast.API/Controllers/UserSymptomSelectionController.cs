using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Services;

namespace MigraineForecast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserSymptomSelectionController : ControllerBase
    {
        private readonly UserSymptomSelectionService _service;

        public UserSymptomSelectionController(UserSymptomSelectionService service)
        {
            _service = service;
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create(UserSymptomSelectionDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
    }
}
