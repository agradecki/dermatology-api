using DermatologyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public class IdempotencyRepository : IIdempotencyRepository
    {
        private readonly ApplicationDbContext _context;

        public IdempotencyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IdempotencyRecord?> GetByKeyAsync(string key)
        {
            return await _context.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);
        }

        public async Task CreateAsync(IdempotencyRecord record)
        {
            _context.IdempotencyRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateResultAsync(string key, string result, string status)
        {
            var record = await _context.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);

            if(record != null)
            {
                record.Result = result;
                record.Status = status;
                record.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateStatusAsync(string key, string status, string? errorMessage = null)
        {
            var record = await _context.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);
            if (record != null)
            {
                record.Status = status;
                record.ErrorMessage = errorMessage;
                record.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
