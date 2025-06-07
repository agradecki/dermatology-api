using DermatologyApi.Exceptions;
using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<Patient> CreateAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            var existingEntity = await _context.Patients.FindAsync(patient.Id);
            if (existingEntity == null)
            { 
                throw new NotFoundException($"Patient with ID {patient.Id} not found");
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(patient);
            await _context.SaveChangesAsync();

            return existingEntity;
        }

        public async Task<Patient> PatchAsync(Patient patient)
        {
            var existingEntity = await _context.Patients.FindAsync(patient.Id);
            if (existingEntity == null)
                throw new NotFoundException($"Patient with ID {patient.Id} not found");

            _context.Entry(existingEntity).CurrentValues.SetValues(patient);
            await _context.SaveChangesAsync();

            return existingEntity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return false;
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
