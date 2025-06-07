using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public interface IDiagnosisService
    {
        Task<Diagnosis> GetDiagnosisEntityByIdAsync(int id);
        Task<IEnumerable<DiagnosisDto>> GetAllDiagnosesAsync();
        Task<DiagnosisDto> GetDiagnosisByIdAsync(int id);
        Task<DiagnosisDto> CreateDiagnosisAsync(DiagnosisCreateDto diagnosisDto, string idempotencyKey);
        Task<DiagnosisDto> CreateDiagnosisWithIdempotencyAsync(DiagnosisCreateDto diagnosisDto, string idempotencyKey);
        Task<DiagnosisDto> UpdateDiagnosisAsync(int id, DiagnosisUpdateDto diagnosisDto, uint expectedXmin);
        Task<bool> DeleteDiagnosisAsync(int id);
    }
}
