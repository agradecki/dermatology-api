using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Exceptions;
using DermatologyApi.Mappers;
using DermatologyApi.Models;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DermatologyApi.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDermatologistRepository _dermatologistRepository;
        private readonly ILesionRepository _lesionRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public DiagnosisService(
            IDiagnosisRepository diagnosisRepository,
            IPatientRepository patientRepository,
            IDermatologistRepository dermatologistRepository,
            ILesionRepository lesionRepository,
            IIdempotencyRepository idempotencyRepository)
        {
            _diagnosisRepository = diagnosisRepository;
            _patientRepository = patientRepository;
            _dermatologistRepository = dermatologistRepository;
            _lesionRepository = lesionRepository;
            _idempotencyRepository = idempotencyRepository;
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

        public async Task<DiagnosisDto> CreateDiagnosisAsync(DiagnosisCreateDto diagnosisDto, string idempotencyKey)
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

            var diagnosis = DiagnosisMapper.MapFromCreateDto(diagnosisDto);
            var createdDiagnosis = await _diagnosisRepository.CreateAsync(diagnosis);

            return DiagnosisMapper.MapToDto(createdDiagnosis);
        }

        public async Task<DiagnosisDto> CreateDiagnosisWithIdempotencyAsync(DiagnosisCreateDto diagnosisDto, string idempotencyKey)
        {
            return await ExecuteWithIdempotencyAsync(idempotencyKey, () => CreateDiagnosisAsync(diagnosisDto, idempotencyKey));
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

        private async Task<T> ExecuteWithIdempotencyAsync<T>(string idempotencyKey, Func<Task<T>> operation)
        {
            var existingOperation = await _idempotencyRepository.GetByKeyAsync(idempotencyKey);
            if (existingOperation != null)
            {
                if (existingOperation.Status == "Completed")
                {
                    return JsonSerializer.Deserialize<T>(existingOperation.Result);
                }

                if (existingOperation.Status == "InProgress")
                {
                    throw new ConflictException("Operation in progress");
                }

                if (existingOperation.Status == "Failed")
                {
                    throw new ConflictException($"Previous operation failed: {existingOperation.ErrorMessage}");
                }
            }

            await _idempotencyRepository.CreateAsync(new IdempotencyRecord
            {
                Key = idempotencyKey,
                Status = "InProgress",
                CreatedAt = DateTime.UtcNow
            });

            try
            {
                var result = await operation();
                await _idempotencyRepository.UpdateResultAsync(idempotencyKey,
                    JsonSerializer.Serialize(result), "Completed");
                return result;
            }
            catch (Exception ex)
            {
                await _idempotencyRepository.UpdateStatusAsync(idempotencyKey, "Failed", ex.Message);
                throw;
            }
        }
    }
}