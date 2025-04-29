using DermatologyApi.Data;
using DermatologyApi.Data.Repositories;
using DermatologyApi.DTOs;
using DermatologyApi.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDermatologistRepository _dermatologistRepository;
        private readonly ApplicationDbContext _dbContext;

        public ConsultationService(IConsultationRepository consultationRepository, IPatientRepository patientRepository, IDermatologistRepository dermatologistRepository, ApplicationDbContext dbContext)
        {
            _consultationRepository = consultationRepository;
            _patientRepository = patientRepository;
            _dermatologistRepository = dermatologistRepository;
            _dbContext = dbContext;
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

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var consultation = ConsultationMapper.MapFromCreateDto(consultationDto);
                consultation = await _consultationRepository.CreateAsync(consultation);

                await transaction.CommitAsync();
                return ConsultationMapper.MapToDto(consultation);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ConsultationDto> UpdateConsultationAsync(int id, ConsultationUpdateDto consultationDto, string etag)
        {
            var consultation = await _consultationRepository.GetByIdAsync(id);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {id} not found");

            if (etag != Convert.ToBase64String(consultation.RowVersion))
                throw new DbUpdateConcurrencyException("The consultation has been modified by another user");

            ConsultationMapper.MapFromUpdateDto(consultationDto, consultation);

            consultation = await _consultationRepository.UpdateAsync(consultation);
            return ConsultationMapper.MapToDto(consultation);
        }

        public async Task DeleteConsultationAsync(int id)
        {
            var consultation = await _consultationRepository.GetByIdAsync(id);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {id} not found");

            await _consultationRepository.DeleteAsync(consultation.Id);
        }

        public async Task TransferConsultationsAsync(TransferRequest[] transfers)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var transfer in transfers)
                {
                    var consultation = await _consultationRepository.GetByIdAsync(transfer.ConsultationId);
                    if (consultation == null)
                        throw new KeyNotFoundException($"Consultation with ID {transfer.ConsultationId} not found");

                    if (await _consultationRepository.IsTimeSlotAvailableAsync(
                        consultation.DermatologistId,
                        transfer.NewDateTime,
                        consultation.Id) == false)
                    {
                        throw new InvalidOperationException($"The time slot {transfer.NewDateTime} is not available");
                    }

                    consultation.ConsultationDate = transfer.NewDateTime;
                    await _consultationRepository.UpdateAsync(consultation);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
