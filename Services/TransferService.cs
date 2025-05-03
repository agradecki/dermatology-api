using DermatologyApi.Data;
using DermatologyApi.Data.Repositories;
using DermatologyApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
            if (transfers == null || transfers.Length == 0)
            {
                throw new ValidationException("No transfer requests provided");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                foreach (var transfer in transfers)
                {
                    var consultation = await _consultationRepository.GetByIdAsync(transfer.ConsultationId);
                    if (consultation == null)
                    {
                        throw new NotFoundException($"Consultation with ID {transfer.ConsultationId} not found.");
                    }

                    if (!await _consultationRepository.IsTimeSlotAvailableAsync(
                        consultation.DermatologistId,
                        transfer.NewDateTime,
                        consultation.Id))
                    {
                        throw new ConflictException($"The time slot {transfer.NewDateTime} is already booked for dermatologist ID {consultation.DermatologistId}");
                    }

                    consultation.ConsultationDate = transfer.NewDateTime;

                    try
                    { 
                        await _consultationRepository.UpdateAsync(consultation);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw new PreconditionFailedException($"Consultation with ID {transfer.ConsultationId} has been modified since it was last retrieved");
                    }
                    catch (DbUpdateException ex)
                    {
                        throw new ConflictException($"Unable to update consultation: {ex.Message}");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (NotFoundException ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (ConflictException ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (PreconditionFailedException ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ConflictException($"Failed to transfer consultations: {ex.Message}");
            }
        }
    }
}
