using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IConsultationRepository _consultationRepository;

        public PatientService(IPatientRepository patientRepository, IDiagnosisRepository diagnosisRepository, IConsultationRepository consultationRepository)
        {
            _patientRepository = patientRepository;
            _diagnosisRepository = diagnosisRepository;
            _consultationRepository = consultationRepository;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return patients.Select(PatientMapper.MapToDto);
        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return null;

            return PatientMapper.MapToDto(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(PatientCreateDto patientDto)
        {
            var patient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                PhoneNumber = patientDto.PhoneNumber,
                Email = patientDto.Email,
                Address = patientDto.Address,
            };

            var createdPatient = await _patientRepository.CreateAsync(patient);
            return PatientMapper.MapToDto(createdPatient);
        }

        public async Task<PatientDto> UpdatePatientAsync(int id, PatientUpdateDto patientDto, byte[] ETag)
        {
            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
                return null;

            existingPatient.FirstName = patientDto.FirstName;
            existingPatient.LastName = patientDto.LastName;
            existingPatient.DateOfBirth = patientDto.DateOfBirth;
            existingPatient.PhoneNumber = patientDto.PhoneNumber;
            existingPatient.Email = patientDto.Email;
            existingPatient.Address = patientDto.Address;
            existingPatient.RowVersion = ETag;

            var updatedPatient = await _patientRepository.UpdateAsync(existingPatient);
            if (updatedPatient == null)
                return null;

            return PatientMapper.MapToDto(updatedPatient);
        }

        public async Task<PatientDto> PatchPatientAsync(int id, PatientPatchDto patientDto, byte[] ETag)
        {
            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
                return null;

            if (patientDto.FirstName != null)
                existingPatient.FirstName = patientDto.FirstName;

            if (patientDto.LastName != null)
                existingPatient.LastName = patientDto.LastName;

            if (patientDto.DateOfBirth.HasValue)
                existingPatient.DateOfBirth = patientDto.DateOfBirth.Value;

            if (patientDto.PhoneNumber != null)
                existingPatient.PhoneNumber = patientDto.PhoneNumber;

            if (patientDto.Email != null)
                existingPatient.Email = patientDto.Email;

            if (patientDto.Address != null)
                existingPatient.Address = patientDto.Address;

            existingPatient.RowVersion = ETag;

            var updatedPatient = await _patientRepository.PatchAsync(existingPatient);
            if (updatedPatient == null)
                return null;

            return PatientMapper.MapToDto(updatedPatient);
        }

        public async Task<IEnumerable<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return null;

            var diagnoses = await _diagnosisRepository.GetByPatientIdAsync(patientId);
            return diagnoses.Select(DiagnosisMapper.MapToDto);
        }

        public async Task<IEnumerable<ConsultationDto>> GetPatientConsultationsAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
                return null;

            var consultations = await _consultationRepository.GetByPatientIdAsync(patientId);
            return consultations.Select(ConsultationMapper.MapToDto);
        }


        public async Task<bool> DeletePatientAsync(int id)
        {
            return await _patientRepository.DeleteAsync(id);
        }
    }
}
