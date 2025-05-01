using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public interface IPatientService
    {
        Task<Patient> GetPatientEntityByIdAsync(int id);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
        Task<PatientDto> CreatePatientAsync(PatientCreateDto patientDto);
        Task<PatientDto> UpdatePatientAsync(int id, PatientUpdateDto patientDto, byte[] rowVersion);
        Task<PatientDto> PatchPatientAsync(int id, PatientPatchDto patientDto);
        Task<bool> DeletePatientAsync(int id);
        Task<IEnumerable<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId);
        Task<IEnumerable<ConsultationDto>> GetPatientConsultationsAsync(int patientId);

    }
}
