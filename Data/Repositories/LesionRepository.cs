using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public interface ILesionRepository
    {
        Task<IEnumerable<Lesion>> GetAllAsync();
        Task<Lesion> GetByIdAsync(int id);
        Task<Lesion> CreateAsync(Lesion lesion);
        Task<Lesion> UpdateAsync(Lesion lesion);
        Task<Lesion> PatchAsync(Lesion lesion);
        Task<bool> DeleteAsync(int id);
    }

    public class LesionRepository : ILesionRepository
    {
        private readonly ApplicationDbContext _context;

        public LesionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lesion>> GetAllAsync()
        {
            return await _context.Lesions
                .Include(l => l.Patient)
                .ToListAsync();
        }

        public async Task<Lesion> GetByIdAsync(int id)
        {
            return await _context.Lesions
                .Include(l => l.Patient)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lesion> CreateAsync(Lesion lesion)
        {
            _context.Lesions.Add(lesion);
            await _context.SaveChangesAsync();
            return lesion;
        }

        public async Task<Lesion> UpdateAsync(Lesion lesion)
        {
            _context.Entry(lesion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LesionExists(lesion.Id))
                {
                    return null;
                }
                throw;
            }
            return lesion;
        }

        public async Task<Lesion> PatchAsync(Lesion lesion)
        {
            _context.Entry(lesion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LesionExists(lesion.Id))
                {
                    return null;
                }
                throw;
            }
            return lesion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lesion = await _context.Lesions.FindAsync(id);
            if (lesion == null)
                return false;

            _context.Lesions.Remove(lesion);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> LesionExists(int id)
        {
            return await _context.Lesions.AnyAsync(e => e.Id == id);
        }
    }
}
