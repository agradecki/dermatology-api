using DermatologyApi.DTOs;

namespace DermatologyApi.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<PatientDto> CreatePatientAsync(PatientCreateDto patientDto);
        Task<PatientDto> UpdatePatientAsync(int id, PatientUpdateDto patientDto, byte[] ETag);
        Task<PatientDto> PatchPatientAsync(int id, PatientPatchDto patientDto, byte[] ETag);
        Task<bool> DeletePatientAsync(int id);
        Task<IEnumerable<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId);
        Task<IEnumerable<ConsultationDto>> GetPatientConsultationsAsync(int patientId);

    }
}
