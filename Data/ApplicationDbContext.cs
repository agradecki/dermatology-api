using DermatologyApi.Models;
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
        public DbSet<Transfer> Transfers { get; set; }

        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Xmin)
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Dermatologist>()
                .Property(d => d.Xmin)
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Lesion>()
                .Property(l => l.Xmin)
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Diagnosis>()
                .Property(d => d.Xmin)
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Consultation>()
                .Property(c => c.Xmin)
                .IsConcurrencyToken()
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Lesion>()
                .HasMany(l => l.Diagnoses)
                .WithOne(d => d.Lesion)
                .HasForeignKey(d => d.LesionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<IdempotencyRecord>(entity =>
            {
                entity.HasKey(e => e.Key);
                entity.Property(e => e.Key).HasMaxLength(255);
                entity.Property(e => e.Status).HasMaxLength(50);
            });
        }
    }
}
