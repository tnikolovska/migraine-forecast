using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Services;

namespace MigraineForecast.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SymptomController : ControllerBase
    {
        private readonly SymptomService _service;

        public SymptomController(SymptomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SymptomDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Data is missing.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            await _service.CreateAsync(dto);
            return Ok("Symptom created");
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var symptom = await _service.GetByIdAsync(id);

            if (symptom == null)
                return NotFound();

            return Ok(symptom);
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SymptomDto dto)
        {
            Console.WriteLine($"PUT HIT: {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);

            Console.WriteLine($"UPDATED RESULT: {updated}");

            if (!updated)
                return NotFound();

            return Ok();
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
    }
}
