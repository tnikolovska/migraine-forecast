using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;

namespace MigraineForecast.API.Services
{
    public class HealthConditionService
    {
        private readonly ApplicationDbContext _context;

        public HealthConditionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL
        public async Task<List<HealthConditionResponseDto>> GetAllAsync()
        {
            var conditions = await _context.HealthConditions
                .Include(h => h.Symptoms)
                .ToListAsync();

            return conditions.Select(h => new HealthConditionResponseDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Symptoms = h.Symptoms.Select(s => new SymptomDto
                {
                    Name = s.Name,
                    Description = s.Description,
                    HealthConditionId = s.HealthConditionId,
                    Type = s.Type.ToString()
                }).ToList()
            }).ToList();
        }

        // ✅ GET BY ID
        public async Task<HealthConditionResponseDto?> GetByIdAsync(long id)
        {
            var condition = await _context.HealthConditions
                .Include(h => h.Symptoms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (condition == null)
                return null;

            return new HealthConditionResponseDto
            {
               
                Name = condition.Name,
                Description = condition.Description,
                Symptoms = condition.Symptoms.Select(s => new SymptomDto
                {
                    Name = s.Name,
                    Description = s.Description,
                    HealthConditionId = s.HealthConditionId,
                    Type = s.Type.ToString()
                }).ToList()
            };
        }

        // ✅ CREATE
        public async Task<HealthConditionResponseDto> CreateAsync(HealthConditionDto dto)
        {
            var entity = new HealthCondition
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.HealthConditions.Add(entity);
            await _context.SaveChangesAsync();

            return new HealthConditionResponseDto
            {
                
                Name = entity.Name,
                Description = entity.Description,
                Symptoms = new List<SymptomDto>()
            };
        }

        // ✅ DELETE (optional - admin)
        public async Task<bool> DeleteAsync(long id)
        {
            var condition = await _context.HealthConditions.FindAsync(id);

            if (condition == null)
                return false;

            _context.HealthConditions.Remove(condition);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateAsync(long id, HealthCondition condition)
        {
            if (id != condition.Id) return false;

            _context.Entry(condition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }
    }
}