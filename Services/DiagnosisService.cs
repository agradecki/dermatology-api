using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Exceptions;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Diagnosis> GetDiagnosisEntityByIdAsync(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (diagnosis == null)
            {
                throw new NotFoundException($"Diagnosis with ID {id} not found");
            }

            return diagnosis;
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
            {
                throw new NotFoundException($"Diagnosis with ID {id} not found");
            }

            return DiagnosisMapper.MapToDto(diagnosis);
        }

        public async Task<DiagnosisDto> CreateDiagnosisAsync(DiagnosisCreateDto diagnosisDto)
        {
            var patient = await _patientRepository.GetByIdAsync(diagnosisDto.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {diagnosisDto.PatientId} not found");
            }

            var dermatologist = await _dermatologistRepository.GetByIdAsync(diagnosisDto.DermatologistId);
            if (dermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with ID {diagnosisDto.DermatologistId} not found");
            }

            if (diagnosisDto.LesionId > 0)
            {
                var lesion = await _lesionRepository.GetByIdAsync(diagnosisDto.LesionId);
                if (lesion == null)
                {
                    throw new NotFoundException($"Lesion with ID {diagnosisDto.LesionId} not found");
                }
            }

            // POST ONCE
            var existingDiagnosis = await _diagnosisRepository.GetByPatientDateAndDermatologistAsync(
                diagnosisDto.PatientId, diagnosisDto.DiagnosisDate, diagnosisDto.DermatologistId);

            if (existingDiagnosis != null)
            {
                throw new ConflictException("Diagnosis for this patient, date, and dermatologist already exists.");
            }


            var diagnosis = DiagnosisMapper.MapFromCreateDto(diagnosisDto);
            var createdDiagnosis = await _diagnosisRepository.CreateAsync(diagnosis);

            return DiagnosisMapper.MapToDto(createdDiagnosis);
        }

        public async Task<DiagnosisDto> UpdateDiagnosisAsync(int id, DiagnosisUpdateDto diagnosisDto, byte[] rowVersion)
        {
            var patient = await _patientRepository.GetByIdAsync(diagnosisDto.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {diagnosisDto.PatientId} not found");
            }

            var dermatologist = await _dermatologistRepository.GetByIdAsync(diagnosisDto.DermatologistId);
            if (dermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with ID {diagnosisDto.DermatologistId} not found");
            }

            if (diagnosisDto.LesionId > 0)
            {
                var lesion = await _lesionRepository.GetByIdAsync(diagnosisDto.LesionId);
                if (lesion == null)
                {
                    throw new NotFoundException($"Lesion with ID {diagnosisDto.LesionId} not found");
                }
            }

            var existingDiagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (existingDiagnosis == null)
            {
                throw new NotFoundException($"Diagnosis with ID {id} not found");
            }

            if (!existingDiagnosis.RowVersion.SequenceEqual(rowVersion))
            {
                throw new PreconditionFailedException("The diagnosis has been modified since it was last retrieved");
            }

            DiagnosisMapper.MapFromUpdateDto(existingDiagnosis, diagnosisDto);

            try
            {
                var updatedDiagnosis = await _diagnosisRepository.UpdateAsync(existingDiagnosis);
                return DiagnosisMapper.MapToDto(updatedDiagnosis);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The diagnosis has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update diagnosis: {ex.Message}");
            }
        }

        public async Task<bool> DeleteDiagnosisAsync(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (diagnosis == null)
            {
                throw new NotFoundException($"Diagnosis with ID {id} not found");
            }

            var result = await _diagnosisRepository.DeleteAsync(id);
            if (!result)
            {
                throw new ConflictException($"Unable to delete diagnosis with ID {id}");
            }

            return true;
        }
    }
}