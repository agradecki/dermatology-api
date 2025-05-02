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
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();
            //.IsRowVersion()
            //.ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Dermatologist>()
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();

            modelBuilder.Entity<Lesion>()
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();

            modelBuilder.Entity<Diagnosis>()
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();

            modelBuilder.Entity<Consultation>()
            .Property(p => p.RowVersion)
            .IsConcurrencyToken();

            modelBuilder.Entity<Lesion>()
                .HasMany(l => l.Diagnoses)
                .WithOne(d => d.Lesion)
                .HasForeignKey(d => d.LesionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
