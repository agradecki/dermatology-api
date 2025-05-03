using DermatologyApi.Data;
using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDermatologistRepository _dermatologistRepository;

        public ConsultationService(IConsultationRepository consultationRepository, IPatientRepository patientRepository, IDermatologistRepository dermatologistRepository, ApplicationDbContext dbContext)
        {
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _dermatologistRepository = dermatologistRepository;
        }

        public async Task<Consultation> GetConsultationEntityByIdAsync(int id)
        {
            var consultation = await _consultationRepository.GetByIdAsync(id);
            if (consultation == null)
                return null;

            return consultation;
        }

        public async Task<PagedResult<ConsultationDto>> GetAllConsultationsAsync(int page, int size)
        {
            var (consultations, totalCount) = await _consultationRepository.GetPagedAsync(page, size);

            var result = new PagedResult<ConsultationDto>
            {
                Items = consultations.Select(ConsultationMapper.MapToDto),
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = size,
                PageCount = (int)Math.Ceiling(totalCount / (double)size)
            };

            return result;
        }

        public async Task<ConsultationDto> GetConsultationByIdAsync(int id)
        {
            var consultation = await _consultationRepository.GetByIdAsync(id);
            if (consultation == null)
                return null;

            return ConsultationMapper.MapToDto(consultation);
        }

        public async Task<ConsultationDto> CreateConsultationAsync(ConsultationCreateDto consultationDto)
        {
            var patient = await _patientRepository.GetByIdAsync(consultationDto.PatientId);
            if (patient == null)
                return null;

            var dermatologist = await _dermatologistRepository.GetByIdAsync(consultationDto.DermatologistId);
            if (dermatologist == null)
                return null;

            if (!await _consultationRepository.IsTimeSlotAvailableAsync(
                consultationDto.DermatologistId, consultationDto.ConsultationDate))
            {
                throw new InvalidOperationException("The selected time slot is not available");
            }

            var consultation = new Consultation {
                PatientId = consultationDto.PatientId,
                DermatologistId = consultationDto.DermatologistId,
                ConsultationDate = consultationDto.ConsultationDate,
                Description = consultationDto.Description,
                Patient = patient,
                Dermatologist = dermatologist
            };

            var createdConsultation = await _consultationRepository.CreateAsync(consultation);

            return ConsultationMapper.MapToDto(createdConsultation);
        }

        public async Task<ConsultationDto> UpdateConsultationAsync(int id, ConsultationUpdateDto consultationDto, byte[] rowVersion)
        {
            var existingConsultation = await _consultationRepository.GetByIdAsync(id);
            if (existingConsultation == null)
                throw new KeyNotFoundException($"Consultation with ID {id} not found");

            if (!existingConsultation.RowVersion.SequenceEqual(rowVersion))
                throw new DbUpdateConcurrencyException("ETag does not match. Resource was modified.");

            ConsultationMapper.MapFromUpdateDto(consultationDto, existingConsultation);

            var consultation = await _consultationRepository.UpdateAsync(existingConsultation);
            return ConsultationMapper.MapToDto(consultation);
        }

        public async Task DeleteConsultationAsync(int id)
        {
            var consultation = await _consultationRepository.GetByIdAsync(id);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {id} not found");

            await _consultationRepository.DeleteAsync(consultation.Id);
        }
    }
}
