using DermatologyApi.DTOs;

namespace DermatologyApi.Services
{
    public interface IDermatologistService
    {
        Task<IEnumerable<DermatologistDto>> GetAllDermatologistsAsync();
        Task<DermatologistDto> GetDermatologistByIdAsync(int id);
        Task<DermatologistDto> CreateDermatologistAsync(DermatologistCreateDto dermatologistDto);
        Task<DermatologistDto> UpdateDermatologistAsync(int id, DermatologistUpdateDto dermatologistDto, byte[] rowVersion);
        Task<DermatologistDto> PatchDermatologistAsync(int id, DermatologistPatchDto dermatologistDto, byte[] rowVersion);
        Task<bool> DeleteDermatologistAsync(int id);
        Task<IEnumerable<DiagnosisDto>> GetDermatologistDiagnosesAsync(int dermatologistId);
        Task<IEnumerable<ConsultationDto>> GetDermatologistConsultationsAsync(int dermatologistId);

    }
}
