using DermatologyAPI.Models;

namespace DermatologyApi.Data.Repositories
{
    public interface IDiagnosisRepository
    {
        Task<IEnumerable<Diagnosis>> GetAllAsync();
        Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Diagnosis>> GetByDermatologistIdAsync(int dermatologistId);
        Task<Diagnosis> GetByIdAsync(int id);
        Task<Diagnosis> CreateAsync(Diagnosis diagnosis);
        Task<Diagnosis> UpdateAsync(Diagnosis diagnosis);
        Task<bool> DeleteAsync(int id);
        Task<bool> DiagnosisExistsAsync(int patientId, int dermatologistId, DateTime date);
        Task<Diagnosis> GetByPatientDateAndDermatologistAsync(int patientId, DateTime date, int dermatologistId);
    }
}
