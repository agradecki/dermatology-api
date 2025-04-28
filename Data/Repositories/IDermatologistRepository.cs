using DermatologyAPI.Models;

namespace DermatologyApi.Data.Repositories
{
    public interface IDermatologistRepository
    {
        Task<IEnumerable<Dermatologist>> GetAllAsync();
        Task<Dermatologist> GetByIdAsync(int id);
        Task<Dermatologist> CreateAsync(Dermatologist dermatologist);
        Task<Dermatologist> UpdateAsync(Dermatologist dermatologist);
        Task<Dermatologist> PatchAsync(Dermatologist dermatologist);
        Task<bool> DeleteAsync(int id);
    }
}
