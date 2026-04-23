using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;

namespace MigraineForecast.API.Services
{
    public class SymptomService
    {
        private readonly ApplicationDbContext _context;

        public SymptomService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL
        public async Task<List<SymptomDto>> GetAllAsync()
        {
            var symptoms = await _context.Symptoms.ToListAsync();

            return symptoms.Select(s => new SymptomDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                HealthConditionId = s.HealthConditionId,
                Type = s.Type.ToString()
            }).ToList();
        }

        // ✅ GET BY ID (optional ако сакаш)
        public async Task<SymptomDto?> GetByIdAsync(long id)
        {
            var s = await _context.Symptoms.FindAsync(id);

            if (s == null)
                return null;

            return new SymptomDto
            {
                Id= s.Id,
                Name = s.Name,
                Description = s.Description,
                HealthConditionId = s.HealthConditionId,
                Type = s.Type.ToString()
            };
        }

        // ✅ CREATE
        public async Task CreateAsync(SymptomDto dto)
        {
            var symptom = new Symptom
            {
                Name = dto.Name,
                Description = dto.Description,
                HealthConditionId = dto.HealthConditionId,
                Type = Enum.Parse<MigraineType>(dto.Type)
                //Type = (MigraineType)dto.Type
            };

            _context.Symptoms.Add(symptom);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, SymptomDto dto)
        {
            var symptom = await _context.Symptoms
                .FirstOrDefaultAsync(s => s.Id == id);

            if (symptom == null)
                return false;
            
            symptom.Name = dto.Name;
            symptom.Description = dto.Description;
            symptom.HealthConditionId = dto.HealthConditionId;
            //symptom.Type = Enum.Parse<MigraineType>(dto.Type);
            if (!Enum.TryParse<MigraineType>(dto.Type, out var parsedType))
            {
                return false; // or throw new Exception("Invalid type");
            }

            symptom.Type = parsedType;
            Console.WriteLine($"DTO TYPE = '{dto.Type}'");

            _context.Symptoms.Update(symptom);
            await _context.SaveChangesAsync();

            return true;
        }



        // ✅ DELETE (admin)
        public async Task<bool> DeleteAsync(long id)
        {
            var symptom = await _context.Symptoms.FindAsync(id);

            if (symptom == null)
                return false;

            _context.Symptoms.Remove(symptom);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}