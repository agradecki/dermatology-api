using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Exceptions;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Patient> GetPatientEntityByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found");
            }

            return patient;
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
            {
                 throw new NotFoundException($"Patient with ID {id} not found");
            }

            return PatientMapper.MapToDto(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(PatientCreateDto patientDto)
        {
            var existingPatients = await _patientRepository.GetAllAsync();
            if (existingPatients.Any(p => p.Email.Equals(patientDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"Email {patientDto.Email} is already in use");
            }

            var patient = PatientMapper.MapFromCreateDto(patientDto);

            var createdPatient = await _patientRepository.CreateAsync(patient);
            return PatientMapper.MapToDto(createdPatient);
        }

        public async Task<PatientDto> UpdatePatientAsync(int id, PatientUpdateDto patientDto, byte[] rowVersion)
        {
            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found");
            }

            if (!existingPatient.RowVersion.SequenceEqual(rowVersion))
            {
                throw new PreconditionFailedException("The patient has been modified since it was last retrieved");
            }

            var existingPatients = await _patientRepository.GetAllAsync();
            if (existingPatients.Any(p => p.Id != id && p.Email.Equals(patientDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException($"Email {patientDto.Email} is already in use by another patient");
            }

            PatientMapper.MapFromUpdateDto(existingPatient, patientDto);

            try
            {
                var updatedPatient = await _patientRepository.UpdateAsync(existingPatient);
                return PatientMapper.MapToDto(updatedPatient);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The patient has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update patient: {ex.Message}");
            }
        }

        public async Task<PatientDto> PatchPatientAsync(int id, PatientPatchDto patientDto)
        {
            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found");
            }

            if (patientDto.Email != null && !existingPatient.Email.Equals(patientDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingPatients = await _patientRepository.GetAllAsync();
                if (existingPatients.Any(p => p.Id != id && p.Email.Equals(patientDto.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ConflictException($"Email {patientDto.Email} is already in use by another patient");
                }
            }

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

            try 
            {
                var updatedPatient = await _patientRepository.PatchAsync(existingPatient);
                return PatientMapper.MapToDto(updatedPatient);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The patient has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update patient: {ex.Message}");
            }
        }

        public async Task<IEnumerable<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found");
            }

            var diagnoses = await _diagnosisRepository.GetByPatientIdAsync(patientId);
            return diagnoses.Select(DiagnosisMapper.MapToDto);
        }

        public async Task<IEnumerable<ConsultationDto>> GetPatientConsultationsAsync(int patientId)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} not found");
            }

            var consultations = await _consultationRepository.GetByPatientIdAsync(patientId);
            return consultations.Select(ConsultationMapper.MapToDto);
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found");
            }

            var result = await _patientRepository.DeleteAsync(id);
            if (!result)
            {
                throw new ConflictException($"Unable to delete patient with ID {id}");
            }

            return true;
        }
    }
}
