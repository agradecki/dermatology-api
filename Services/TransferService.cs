using DermatologyApi.Data;
using DermatologyApi.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using static DermatologyApi.Services.ITransferService;

namespace DermatologyApi.Services
{
    public class TransferService : ITransferService
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly ApplicationDbContext _dbContext;

        public TransferService(IConsultationRepository consultationRepository, ApplicationDbContext dbContext)
        {
            _consultationRepository = consultationRepository;
            _dbContext = dbContext;
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
