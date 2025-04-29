using DermatologyApi.DTOs;

namespace DermatologyApi.Services
{
    public interface IDiagnosisService
    {
        Task<IEnumerable<DiagnosisDto>> GetAllDiagnosesAsync();
        Task<DiagnosisDto> GetDiagnosisByIdAsync(int id);
        Task<DiagnosisDto> CreateDiagnosisAsync(DiagnosisCreateDto diagnosisDto);
        Task<DiagnosisDto> UpdateDiagnosisAsync(int id, DiagnosisUpdateDto diagnosisDto, byte[] rowVersion);
        Task<bool> DeleteDiagnosisAsync(int id);
    }
}
