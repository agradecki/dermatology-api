using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public class DermatologistRepository : IDermatologistRepository
    {
        private readonly ApplicationDbContext _context;

        public DermatologistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dermatologist>> GetAllAsync()
        {
            return await _context.Dermatologists.ToListAsync();
        }

        public async Task<Dermatologist> GetByIdAsync(int id)
        {
            return await _context.Dermatologists.FindAsync(id);
        }

        public async Task<Dermatologist> CreateAsync(Dermatologist dermatologist)
        {
            _context.Dermatologists.Add(dermatologist);
            await _context.SaveChangesAsync();
            return dermatologist;
        }

        public async Task<Dermatologist> UpdateAsync(Dermatologist dermatologist)
        {
            _context.Entry(dermatologist).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return dermatologist;
        }

        public async Task<Dermatologist> PatchAsync(Dermatologist dermatologist)
        {
            _context.Entry(dermatologist).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return dermatologist;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dermatologist = await _context.Dermatologists.FindAsync(id);
            if (dermatologist == null)
            {
                return false;
            }

            _context.Dermatologists.Remove(dermatologist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
