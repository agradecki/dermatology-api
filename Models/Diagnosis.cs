using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DermatologyAPI.Models
{
    public class Diagnosis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int PatientId { get; set; }

        [Required]
        public required int DermatologistId { get; set; }

        [Required]
        public required int LesionId {  get; set; }

        [Required]
        public required DateTime DiagnosisDate { get; set; }

        [Required]
        [StringLength(1000)]
        public required string Description { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [ForeignKey("DermatologistId")]
        public Dermatologist? Dermatologist { get; set; }

        [ForeignKey("LesionId")]
        public Lesion? Lesion { get; set; }
    }
}
