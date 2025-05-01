using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

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
                return null;

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
                return null;

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
                return null;

            if (!existingDermatologist.RowVersion.SequenceEqual(rowVersion))
                throw new DbUpdateConcurrencyException("ETag does not match. Resource was modified.");

            existingDermatologist.FirstName = dermatologistDto.FirstName;
            existingDermatologist.LastName = dermatologistDto.LastName;
            existingDermatologist.LicenseNumber = dermatologistDto.LicenseNumber;
            existingDermatologist.Specialization = dermatologistDto.Specialization;
            existingDermatologist.Email = dermatologistDto.Email;
            existingDermatologist.PhoneNumber = dermatologistDto.PhoneNumber;

            var updatedDermatologist = await _dermatologistRepository.UpdateAsync(existingDermatologist);
            if (updatedDermatologist == null)
                return null;

            return DermatologistMapper.MapToDto(updatedDermatologist);
        }

        public async Task<DermatologistDto> PatchDermatologistAsync(int id, DermatologistPatchDto dermatologistDto)
        {
            var existingDermatologist = await _dermatologistRepository.GetByIdAsync(id);
            if (existingDermatologist == null)
                return null;

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

            var updatedDermatologist = await _dermatologistRepository.PatchAsync(existingDermatologist);
            if (updatedDermatologist == null)
                return null;

            return DermatologistMapper.MapToDto(updatedDermatologist);
        }

        public async Task<bool> DeleteDermatologistAsync(int id)
        {
            return await _dermatologistRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DiagnosisDto>> GetDermatologistDiagnosesAsync(int dermatologistId)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(dermatologistId);
            if (dermatologist == null)
                return null;

            var diagnoses = await _diagnosisRepository.GetByDermatologistIdAsync(dermatologistId);
            return diagnoses.Select(DiagnosisMapper.MapToDto);
        }

        public async Task<IEnumerable<ConsultationDto>> GetDermatologistConsultationsAsync(int dermatologistId)
        {
            var dermatologist = await _dermatologistRepository.GetByIdAsync(dermatologistId);
            if (dermatologist == null)
                return null;

            var consultations = await _consultationRepository.GetByDermatologistIdAsync(dermatologistId);
            return consultations.Select(ConsultationMapper.MapToDto);
        }

    }
}