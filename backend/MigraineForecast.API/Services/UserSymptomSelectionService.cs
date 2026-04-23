using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;

namespace MigraineForecast.API.Services
{
    public class UserSymptomSelectionService
    {
        private readonly ApplicationDbContext _context;

        public UserSymptomSelectionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE
        public async Task<UserSymptomSelectionResponseDto> CreateAsync(UserSymptomSelectionDto dto)
        {
            /*var symptoms = await _context.Symptoms
                .Where(s => dto.SymptomIds.Contains(s.Id))
                .ToListAsync();

            var entity = new UserSymptomSelection
            {
                UserHealthConditionId = dto.UserHealthConditionId,
                MigraineSymptoms = symptoms
            };*/
            // 1. Proveri da li uopšte stižu ID-jevi
            if (dto.SymptomIds == null || !dto.SymptomIds.Any())
            {
                throw new Exception("Nisu poslati ID-jevi simptoma.");
            }

            // 2. Dobavi simptome iz baze
            var symptoms = await _context.Symptoms
                .Where(s => dto.SymptomIds.Contains(s.Id))
                .ToListAsync();

            var entity = new UserSymptomSelection
            {
                UserHealthConditionId = dto.UserHealthConditionId,
                MigraineSymptoms = new List<Symptom>() // Inicijalizuj ovde
            };

            // 3. Dodaj simptome jedan po jedan u entitet
            foreach (var s in symptoms)
            {
                entity.MigraineSymptoms.Add(s);
            }

            _context.UserSymptomSelections.Add(entity);
            await _context.SaveChangesAsync();

            return new UserSymptomSelectionResponseDto
            {
                Id = entity.Id,
                UserHealthConditionId = entity.UserHealthConditionId,
                Symptoms = symptoms.Select(s => new SymptomDto
                {
                    Name = s.Name,
                    Description = s.Description,
                    HealthConditionId = s.HealthConditionId,
                    Type = s.Type.ToString()
                }).ToList()
            };


        }

        // ✅ GET ALL
        public async Task<List<UserSymptomSelectionResponseDto>> GetAllAsync()
        {
            var list = await _context.UserSymptomSelections
                .Include(x => x.MigraineSymptoms)
                .ToListAsync();

            return list.Select(x => new UserSymptomSelectionResponseDto
            {
                Id = x.Id,
                UserHealthConditionId = x.UserHealthConditionId,
                Symptoms = x.MigraineSymptoms.Select(s => new SymptomDto
                {
                    Name = s.Name,
                    Description = s.Description,
                    HealthConditionId = s.HealthConditionId,
                    Type = s.Type.ToString()
                }).ToList()
            }).ToList();
        }
    }
}