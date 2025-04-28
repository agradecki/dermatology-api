using DermatologyApi.DTOs;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public interface IConsultationRepository
    {
        Task<PaginatedResponseDto<Consultation>> GetAllAsync(int page, int size);
        Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Consultation>> GetByDermatologistIdAsync(int dermatologistId);
        Task<Consultation> GetByIdAsync(int id);
        Task<Consultation> CreateAsync(Consultation consultation);
        Task<Consultation> UpdateAsync(Consultation consultation);
        Task<bool> DeleteAsync(int id);
        Task<bool> TransferConsultationsAsync(List<Transfer> transfers);
    }

    public class ConsultationRepository : IConsultationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConsultationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDto<Consultation>> GetAllAsync(int page, int size)
        {
            var query = _context.Consultations
                .Include(c => c.Patient)
                .Include(c => c.Dermatologist)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PaginatedResponseDto<Consultation>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = size
            };
        }

        public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Consultations
                .Include(c => c.Dermatologist)
                .Where(c => c.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Consultation>> GetByDermatologistIdAsync(int dermatologistId)
        {
            return await _context.Consultations
                .Include(c => c.Patient)
                .Where(c => c.DermatologistId == dermatologistId)
                .ToListAsync();
        }

        public async Task<Consultation> GetByIdAsync(int id)
        {
            return await _context.Consultations
                .Include(c => c.Patient)
                .Include(c => c.Dermatologist)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Consultation> CreateAsync(Consultation consultation)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var patient = await _context.Patients.FindAsync(consultation.PatientId);
                    if (patient == null)
                    {
                        throw new KeyNotFoundException($"Nie znaleziono pacjenta o ID {consultation.PatientId}");
                    }

                    var dermatologist = await _context.Dermatologists.FindAsync(consultation.DermatologistId);
                    if (dermatologist == null)
                    {
                        throw new KeyNotFoundException($"Nie znaleziono dermatologa o ID {consultation.DermatologistId}");
                    }

                    _context.Consultations.Add(consultation);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return consultation;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<Consultation> UpdateAsync(Consultation consultation)
        {
            _context.Entry(consultation).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ConsultationExists(consultation.Id))
                {
                    return null;
                }
                throw;
            }
            return consultation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
                return false;

            _context.Consultations.Remove(consultation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferConsultationsAsync(List<Transfer> transfers)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var transfer in transfers)
                    {
                        var consultation = await _context.Consultations.FindAsync(transfer.ConsultationId);
                        if (consultation == null)
                        {
                            throw new KeyNotFoundException($"Nie znaleziono konsultacji o ID {transfer.ConsultationId}");
                        }

                        consultation.ConsultationDate = transfer.NewDateTime;
                        _context.Entry(consultation).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task<bool> ConsultationExists(int id)
        {
            return await _context.Consultations.AnyAsync(e => e.Id == id);
        }
    }
}
