using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public class DiagnosisRepository : IDiagnosisRepository
    {
        private readonly ApplicationDbContext _context;

        public DiagnosisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Diagnosis>> GetAllAsync()
        {
            return await _context.Diagnoses.ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Diagnoses
                .Where(d => d.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetByDermatologistIdAsync(int dermatologistId)
        {
            return await _context.Diagnoses
                .Where(d => d.DermatologistId == dermatologistId)
                .ToListAsync();
        }

        public async Task<Diagnosis> GetByIdAsync(int id)
        {
            return await _context.Diagnoses
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Diagnosis> CreateAsync(Diagnosis diagnosis)
        {
            var exists = await DiagnosisExistsAsync(diagnosis.PatientId, diagnosis.DermatologistId, diagnosis.DiagnosisDate);
            if (exists)
            {
                throw new InvalidOperationException("The diagnosis for this patient, dermatologist and date already exists.");
            }

            _context.Diagnoses.Add(diagnosis);
            await _context.SaveChangesAsync();
            return diagnosis;
        }

        public async Task<Diagnosis> UpdateAsync(Diagnosis diagnosis)
        {
            _context.Entry(diagnosis).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return diagnosis;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(id);
            if (diagnosis == null)
                return false;

            _context.Diagnoses.Remove(diagnosis);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DiagnosisExistsAsync(int patientId, int dermatologistId, DateTime date)
        {
            return await _context.Diagnoses.AnyAsync(d =>
                d.PatientId == patientId &&
                d.DermatologistId == dermatologistId &&
                d.DiagnosisDate.Date == date.Date);
        }
        public async Task<Diagnosis> GetByPatientDateAndDermatologistAsync(int patientId, DateTime date, int dermatologistId)
        {
            return await _context.Diagnoses
                .FirstOrDefaultAsync(d => d.PatientId == patientId
                                       && d.DiagnosisDate.Date == date.Date
                                       && d.DermatologistId == dermatologistId);
        }
    }
}
