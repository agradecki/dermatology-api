using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Exceptions;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Services
{
    public class LesionService : ILesionService
    {
        private readonly ILesionRepository _lesionRepository;
        private readonly IPatientRepository _patientRepository;

        public LesionService(ILesionRepository lesionRepository, IPatientRepository petientRepository)
        {
            _lesionRepository = lesionRepository;
            _patientRepository = petientRepository;
        }

        public async Task<Lesion> GetLesionEntityByIdAsync(int id)
        {
            var lesion = await _lesionRepository.GetByIdAsync(id);
            if (lesion == null)
            {
                throw new NotFoundException($"Lesion with id {id} not found.");
            }

            return lesion;
        }

        public async Task<IEnumerable<LesionDto>> GetAllLesionsAsync()
        {
            var lesions = await _lesionRepository.GetAllAsync();
            return lesions.Select(LesionMapper.MapToDto);
        }

        public async Task<LesionDto> GetLesionByIdAsync(int id)
        {
            var lesion = await _lesionRepository.GetByIdAsync(id);
            if (lesion == null)
            {
                throw new NotFoundException($"Lesion with id {id} not found.");
            }

            return LesionMapper.MapToDto(lesion);
        }

        public async Task<LesionDto> CreateLesionAsync(LesionCreateDto lesionDto)
        {
            var patient = await _patientRepository.GetByIdAsync(lesionDto.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with id {lesionDto.PatientId} not found.");
            }

            var lesion = LesionMapper.MapFromCreateDto(lesionDto);
            lesion.Patient = patient;

            var createdLesion = await _lesionRepository.CreateAsync(lesion);
            return LesionMapper.MapToDto(createdLesion);
        }

        public async Task<LesionDto> UpdateLesionAsync(int id, LesionUpdateDto lesionDto, byte[] rowVersion)
        {
            var patient = await _patientRepository.GetByIdAsync(lesionDto.PatientId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with id {lesionDto.PatientId} not found.");
            }

            var existingLesion = await _lesionRepository.GetByIdAsync(id);
            if (existingLesion == null)
            {
                throw new NotFoundException($"Lesion with id {id} not found.");
            }

            if (!existingLesion.RowVersion.SequenceEqual(rowVersion))
            {
                throw new PreconditionFailedException("The lesion has been modified since it was last retrieved");
            }

            LesionMapper.MapFromUpdateDto(lesionDto, existingLesion);

            try
            {
                var updatedLesion = await _lesionRepository.UpdateAsync(existingLesion);
                return LesionMapper.MapToDto(updatedLesion);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The lesion has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update lesion: {ex.Message}");
            }

        }

        public async Task<LesionDto> PatchLesionAsync(int id, LesionPatchDto lesionDto)
        {
            var existingLesion = await _lesionRepository.GetByIdAsync(id);
            if (existingLesion == null)
            {
                throw new NotFoundException($"Lesion with id {id} not found.");
            }

            if (lesionDto.PatientId.HasValue)
            {
                var patient = await _patientRepository.GetByIdAsync(lesionDto.PatientId.Value);
                if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found");
            }

                existingLesion.PatientId = lesionDto.PatientId.Value;
            }

            if (lesionDto.Location != null)
                existingLesion.Location = lesionDto.Location;

            if (lesionDto.DiscoveryDate.HasValue)
                existingLesion.DiscoveryDate = lesionDto.DiscoveryDate.Value;

            if (lesionDto.Description != null)
                existingLesion.Description = lesionDto.Description;

            try
            {
                var updatedLesion = await _lesionRepository.PatchAsync(existingLesion);
                return LesionMapper.MapToDto(updatedLesion);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new PreconditionFailedException("The lesion has been modified since it was last retrieved");
            }
            catch (DbUpdateException ex)
            {
                throw new ConflictException($"Unable to update lesion: {ex.Message}");
            }
        }

        public async Task<bool> DeleteLesionAsync(int id)
        {
            var lesion = await _lesionRepository.GetByIdAsync(id);
            if (lesion == null)
            {
                throw new NotFoundException($"Lesion with ID {id} not found");
            }

            var result = await _lesionRepository.DeleteAsync(id);
            if (!result)
            {
                throw new ConflictException($"Unable to delete lesion with ID {id}");
            }

            return true;
        }
    }
}
