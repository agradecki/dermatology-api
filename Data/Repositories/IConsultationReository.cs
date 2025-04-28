using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Data.Repositories
{
    public interface IConsultationRepository
    {
        Task<PaginatedResponseDto<Consultation>> GetAllAsync(int page, int size);
        Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Consultation>> GetByDermatologistIdAsync(int dermatologistId);
        Task<Consultation> GetByIdAsync(int id);
        Task<Consultation> CreateAsync(Consultation consultation);
        Task<Consultation> UpdateAsync(Consultation consultation);
        Task<bool> DeleteAsync(int id);
        Task<bool> TransferConsultationsAsync(List<Transfer> transfers);
    }
}
