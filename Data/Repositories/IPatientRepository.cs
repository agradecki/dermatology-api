using DermatologyAPI.Models;

namespace DermatologyApi.Data.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<Patient> PatchAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
    }
}
