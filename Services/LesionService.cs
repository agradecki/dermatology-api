using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Services
{
    public class LesionService : ILesionService
    {
        private readonly ILesionRepository _lesionRepository;
        private readonly IPatientRepository _petientRepository;

        public LesionService(ILesionRepository lesionRepository, IPatientRepository petientRepository)
        {
            _lesionRepository = lesionRepository;
            _petientRepository = petientRepository;
        }

        public async Task<Lesion> GetLesionEntityByIdAsync(int id)
        {
            var lesion = await _lesionRepository.GetByIdAsync(id);
            if (lesion == null)
                return null;

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
                return null;

            return LesionMapper.MapToDto(lesion);
        }

        public async Task<LesionDto> CreateLesionAsync(LesionCreateDto lesionDto)
        {
            var patient = await _petientRepository.GetByIdAsync(lesionDto.PatientId);
            if (patient == null)
                return null;

            var lesion = new Lesion
            {
                PatientId = lesionDto.PatientId,
                Location = lesionDto.Location,
                DiscoveryDate = lesionDto.DiscoveryDate,
                Description = lesionDto.Description,
                Patient = patient,
            };

            var createdLesion = await _lesionRepository.CreateAsync(lesion);
            return LesionMapper.MapToDto(createdLesion);
        }

        public async Task<LesionDto> UpdateLesionAsync(int id, LesionUpdateDto lesionDto, byte[] rowVersion)
        {
            var patient = await _petientRepository.GetByIdAsync(lesionDto.PatientId);
            if (patient == null)
                return null;

            var existingLesion = await _lesionRepository.GetByIdAsync(id);
            if (existingLesion == null)
                return null;

            if (!existingLesion.RowVersion.SequenceEqual(rowVersion))
                throw new DbUpdateConcurrencyException("ETag does not match. Resource was modified.");

            existingLesion.PatientId = lesionDto.PatientId;
            existingLesion.Location = lesionDto.Location;
            existingLesion.DiscoveryDate = lesionDto.DiscoveryDate;
            existingLesion.Description = lesionDto.Description;

            var updatedLesion = await _lesionRepository.UpdateAsync(existingLesion);
            if (updatedLesion == null)
                return null;

            return LesionMapper.MapToDto(updatedLesion);
        }

        public async Task<LesionDto> PatchLesionAsync(int id, LesionPatchDto lesionDto)
        {
            var existingLesion = await _lesionRepository.GetByIdAsync(id);
            if (existingLesion == null)
                return null;

            if (lesionDto.PatientId.HasValue)
            {
                var patient = await _petientRepository.GetByIdAsync(lesionDto.PatientId.Value);
                if (patient == null)
                    return null;

                existingLesion.PatientId = lesionDto.PatientId.Value;
            }

            if (lesionDto.Location != null)
                existingLesion.Location = lesionDto.Location;

            if (lesionDto.DiscoveryDate.HasValue)
                existingLesion.DiscoveryDate = lesionDto.DiscoveryDate.Value;

            if (lesionDto.Description != null)
                existingLesion.Description = lesionDto.Description;

            var updatedLesion = await _lesionRepository.PatchAsync(existingLesion);
            if (updatedLesion == null)
                return null;

            return LesionMapper.MapToDto(updatedLesion);
        }

        public async Task<bool> DeleteLesionAsync(int id)
        {
            return await _lesionRepository.DeleteAsync(id);
        }
    }
}
