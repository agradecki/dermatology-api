using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public interface IDermatologistService
    {
        Task<Dermatologist> GetDermatologistEntityByIdAsync(int id);
        Task<IEnumerable<DermatologistDto>> GetAllDermatologistsAsync();
        Task<DermatologistDto> GetDermatologistByIdAsync(int id);
        Task<DermatologistDto> CreateDermatologistAsync(DermatologistCreateDto dermatologistDto);
        Task<DermatologistDto> UpdateDermatologistAsync(int id, DermatologistUpdateDto dermatologistDto, byte[] rowVersion);
        Task<DermatologistDto> PatchDermatologistAsync(int id, DermatologistPatchDto dermatologistDto);
        Task<bool> DeleteDermatologistAsync(int id);
        Task<IEnumerable<DiagnosisDto>> GetDermatologistDiagnosesAsync(int dermatologistId);
        Task<IEnumerable<ConsultationDto>> GetDermatologistConsultationsAsync(int dermatologistId);

    }
}
