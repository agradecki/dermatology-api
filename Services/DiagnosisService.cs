using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDermatologistRepository _dermatologistRepository;
        private readonly ILesionRepository _lesionRepository;

        public DiagnosisService(
            IDiagnosisRepository diagnosisRepository,
            IPatientRepository patientRepository,
            IDermatologistRepository dermatologistRepository,
            ILesionRepository lesionRepository)
        {
            _diagnosisRepository = diagnosisRepository;
            _patientRepository = patientRepository;
            _dermatologistRepository = dermatologistRepository;
            _lesionRepository = lesionRepository;
        }

        public async Task<IEnumerable<DiagnosisDto>> GetAllDiagnosesAsync()
        {
            var diagnoses = await _diagnosisRepository.GetAllAsync();
            return diagnoses.Select(DiagnosisMapper.MapToDto);
        }

        public async Task<DiagnosisDto> GetDiagnosisByIdAsync(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (diagnosis == null)
                return null;

            return DiagnosisMapper.MapToDto(diagnosis);
        }

        public async Task<DiagnosisDto> CreateDiagnosisAsync(DiagnosisCreateDto diagnosisDto)
        {
            var patient = await _patientRepository.GetByIdAsync(diagnosisDto.PatientId);
            if (patient == null)
                return null;

            var dermatologist = await _dermatologistRepository.GetByIdAsync(diagnosisDto.DermatologistId);
            if (dermatologist == null)
                return null;

            if (diagnosisDto.LesionId > 0)
            {
                var lesion = await _lesionRepository.GetByIdAsync(diagnosisDto.LesionId);
                if (lesion == null)
                    return null;
            }

            var diagnosis = new Diagnosis
            {
                PatientId = diagnosisDto.PatientId,
                DermatologistId = diagnosisDto.DermatologistId,
                LesionId = diagnosisDto.LesionId,
                DiagnosisDate = diagnosisDto.DiagnosisDate,
                Description = diagnosisDto.Description,
            };

            var createdDiagnosis = await _diagnosisRepository.CreateAsync(diagnosis);
            return DiagnosisMapper.MapToDto(createdDiagnosis);
        }

        public async Task<DiagnosisDto> UpdateDiagnosisAsync(int id, DiagnosisUpdateDto diagnosisDto, byte[] rowVersion)
        {
            var patient = await _patientRepository.GetByIdAsync(diagnosisDto.PatientId);
            if (patient == null)
                return null;

            var dermatologist = await _dermatologistRepository.GetByIdAsync(diagnosisDto.DermatologistId);
            if (dermatologist == null)
                return null;

            if (diagnosisDto.LesionId > 0)
            {
                var lesion = await _lesionRepository.GetByIdAsync(diagnosisDto.LesionId);
                if (lesion == null)
                    return null;
            }

            var existingDiagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (existingDiagnosis == null)
                return null;

            existingDiagnosis.PatientId = diagnosisDto.PatientId;
            existingDiagnosis.DermatologistId = diagnosisDto.DermatologistId;
            existingDiagnosis.LesionId = diagnosisDto.LesionId;
            existingDiagnosis.DiagnosisDate = diagnosisDto.DiagnosisDate;
            existingDiagnosis.Description = diagnosisDto.Description;
            existingDiagnosis.RowVersion = rowVersion;

            var updatedDiagnosis = await _diagnosisRepository.UpdateAsync(existingDiagnosis);
            if (updatedDiagnosis == null)
                return null;

            return DiagnosisMapper.MapToDto(updatedDiagnosis);
        }

        public async Task<bool> DeleteDiagnosisAsync(int id)
        {
            return await _diagnosisRepository.DeleteAsync(id);
        }
    }
}