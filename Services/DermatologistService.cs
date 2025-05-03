using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Exceptions;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DermatologyApi.Services
{
    public class DermatologistService : IDermatologistService
    {
        private readonly IDermatologistRepository _dermatologistRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IConsultationRepository _consultationRepository;

        public DermatologistService(IDermatologistRepository dermatologistRepository, IDiagnosisRepository diagnosisRepository, IConsultationRepository consultationRepository)
        {
            _dermatologistRepository = dermatologistRepository;
            _diagnosisRepository = diagnosisRepository;
            _consultationRepository = consultationRepository;
        }

        public async Task<Dermatologist> GetDermatologistEntityByIdAsync(int id)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (dermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with id {id} not found.");
            }

            return dermatologist;
        }

        public async Task<IEnumerable<DermatologistDto>> GetAllDermatologistsAsync()
        {
            var dermatologists = await _dermatologistRepository.GetAllAsync();
            return dermatologists.Select(DermatologistMapper.MapToDto);
        }

        public async Task<DermatologistDto> GetDermatologistByIdAsync(int id)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (dermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with id {id} not found.");
            }

            return DermatologistMapper.MapToDto(dermatologist);
        }

        public async Task<DermatologistDto> CreateDermatologistAsync(DermatologistCreateDto dermatologistDto)
        {
            var dermatologist = new Dermatologist
            {
                FirstName = dermatologistDto.FirstName,
                LastName = dermatologistDto.LastName,
                LicenseNumber = dermatologistDto.LicenseNumber,
                Specialization = dermatologistDto.Specialization,
                Email = dermatologistDto.Email,
                PhoneNumber = dermatologistDto.PhoneNumber
            };

            var createdDermatologist = await _dermatologistRepository.CreateAsync(dermatologist);
            return DermatologistMapper.MapToDto(createdDermatologist);
        }

        public async Task<DermatologistDto> UpdateDermatologistAsync(int id, DermatologistUpdateDto dermatologistDto, byte[] rowVersion)
        {
            var existingDermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (existingDermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with id {id} not found.");
            }

            if (!existingDermatologist.RowVersion.SequenceEqual(rowVersion))
            {
                throw new PreconditionFailedException("The dermatologist has been modified since it was last retrieved");
            }

            existingDermatologist.FirstName = dermatologistDto.FirstName;
            existingDermatologist.LastName = dermatologistDto.LastName;
            existingDermatologist.LicenseNumber = dermatologistDto.LicenseNumber;
            existingDermatologist.Specialization = dermatologistDto.Specialization;
            existingDermatologist.Email = dermatologistDto.Email;
            existingDermatologist.PhoneNumber = dermatologistDto.PhoneNumber;

            try
            {
                var updatedDermatologist = await _dermatologistRepository.UpdateAsync(existingDermatologist);
                return DermatologistMapper.MapToDto(updatedDermatologist);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The dermatologist has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update dermatologist: {ex.Message}");
            }
        }

        public async Task<DermatologistDto> PatchDermatologistAsync(int id, DermatologistPatchDto dermatologistDto)
        {
            var existingDermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (existingDermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with id {id} not found.");
            }

            if (dermatologistDto.FirstName != null)
                existingDermatologist.FirstName = dermatologistDto.FirstName;

            if (dermatologistDto.LastName != null)
                existingDermatologist.LastName = dermatologistDto.LastName;

            if (dermatologistDto.LicenseNumber != null)
                existingDermatologist.LicenseNumber = dermatologistDto.LicenseNumber;

            if (dermatologistDto.Specialization != null)
                existingDermatologist.Specialization = dermatologistDto.Specialization;

            if (dermatologistDto.Email != null)
                existingDermatologist.Email = dermatologistDto.Email;

            if (dermatologistDto.PhoneNumber != null)
                existingDermatologist.PhoneNumber = dermatologistDto.PhoneNumber;

            try
            {
                var updatedDermatologist = await _dermatologistRepository.PatchAsync(existingDermatologist);
                return DermatologistMapper.MapToDto(updatedDermatologist);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The dermatologist has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update dermatologist: {ex.Message}");
            }
        }

        public async Task<bool> DeleteDermatologistAsync(int id)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (dermatologist == null)
            {
                throw new NotFoundException($"Dermatologist with ID {id} not found");
            }

            var result = await _dermatologistRepository.DeleteAsync(id);
            if (!result)
            {
                throw new ConflictException($"Unable to delete dermatologist with ID {id}");
            }

            return true;
        }

        public async Task<IEnumerable<DiagnosisDto>> GetDermatologistDiagnosesAsync(int dermatologistId)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(dermatologistId);
            if (dermatologist == null)
                if (dermatologist == null)
                {
                    throw new NotFoundException($"Dermatologist with ID {dermatologistId} not found");
                }

            var diagnoses = await _diagnosisRepository.GetByDermatologistIdAsync(dermatologistId);
            return diagnoses.Select(DiagnosisMapper.MapToDto);
        }

        public async Task<IEnumerable<ConsultationDto>> GetDermatologistConsultationsAsync(int dermatologistId)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(dermatologistId);
            if (dermatologist == null)
                if (dermatologist == null)
                {
                    throw new NotFoundException($"Dermatologist with ID {dermatologistId} not found");
                }

            var consultations = await _consultationRepository.GetByDermatologistIdAsync(dermatologistId);
            return consultations.Select(ConsultationMapper.MapToDto);
        }

    }
}