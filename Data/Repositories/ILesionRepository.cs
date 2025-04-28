using DermatologyAPI.Models;

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
}
