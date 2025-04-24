using DermatologyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DermatologyApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Dermatologist> Dermatologists { get; set; }
        public DbSet<Lesion> Lesions { get; set; }
        public DbSet<Diagnosis> Diagnoses {  get; set; }
        public DbSet<Consultation> Consultations {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Diagnoses)
                .WithOne(d => d.Patient)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Consultations)
                .WithOne(dc=> c.Patient)
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dermatologist>()
                .HasMany(d => d.Diagnoses)
                .WithOne(di => di.Dermatologist)
                .HasForeignKey(di => di.DermatologistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dermatologist>()
                .HasMany(d => d.Consultations)
                .WithOne(c => c.Dermatologist)
                .HasForeignKey(c => c.DermatologistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesion>()
                .HasMany(l => l.Diagnoses)
                .WithOne(d => d.Lesion)
                .HasForeignKey(d => d.LesionId)
                .OnDelete(DeleteBehavior.SetNull)
        }
    }
}
