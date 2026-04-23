using Microsoft.EntityFrameworkCore;
using MigraineForecast.API.Data;
using MigraineForecast.API.DTOs;
using MigraineForecast.API.Models;

namespace MigraineForecast.API.Services
{
    public class UserHealthConditionService
    {
        private readonly ApplicationDbContext _context;

        public UserHealthConditionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE (assign condition to user)
        public async Task<UserHealthConditionResponseDto> CreateAsync(string userId, UserHealthConditionDto dto)
        {
            /* var exists = await _context.UserHealthConditions
                 .AnyAsync(x => x.UserId == userId && x.HealthConditionId == dto.HealthConditionId);

             if (exists)
                 return null;

             var entity = new UserHealthCondition
             {
                 UserId = userId,
                 HealthConditionId = dto.HealthConditionId
             };

             _context.UserHealthConditions.Add(entity);
             await _context.SaveChangesAsync();

             return new UserHealthConditionResponseDto
             {
                 Id = entity.Id,
                 UserId = entity.UserId,
                 HealthConditionId = entity.HealthConditionId
             };*/

            var existing = await _context.UserHealthConditions
             .FirstOrDefaultAsync(x =>
           x.UserId == userId &&
           x.HealthConditionId == dto.HealthConditionId);

            // If exists → just return it (no error)
            if (existing != null)
            {
                return new UserHealthConditionResponseDto
                {
                    Id = existing.Id,
                    UserId = existing.UserId,
                    HealthConditionId = existing.HealthConditionId
                };
            }

            var entity = new UserHealthCondition
            {
                UserId = userId,
                HealthConditionId = dto.HealthConditionId
            };

            _context.UserHealthConditions.Add(entity);
            await _context.SaveChangesAsync();

            return new UserHealthConditionResponseDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                HealthConditionId = entity.HealthConditionId
            };



        }

        // ✅ GET USER CONDITIONS
        public async Task<List<UserHealthConditionResponseDto>> GetByUserAsync(string userId)
        {
            var list = await _context.UserHealthConditions
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return list.Select(x => new UserHealthConditionResponseDto
            {
                Id = x.Id,
                UserId = x.UserId,
                HealthConditionId = x.HealthConditionId
            }).ToList();
        }

        // ✅ CHECK (ова ти треба за Forecast)
        public async Task<bool> HasConditionAsync(string userId)
        {
            return await _context.UserHealthConditions
                .AnyAsync(x => x.UserId == userId);
        }

        // ✅ DELETE
        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.UserHealthConditions.FindAsync(id);

            if (entity == null)
                return false;

            _context.UserHealthConditions.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}