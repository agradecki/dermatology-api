using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DermatologyAPI.Models
{
    public class Consultation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int PatientId { get; set; }

        [Required]
        public required int DermatologistId { get; set; }

        [Required]
        public required DateTime ConsultationDate { get; set; }



        [StringLength(1000)]
        public string? Description { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [ForeignKey("PatientId")]
        public required Patient Patient { get; set; }

        [ForeignKey("DermatologistId")]
        public required Dermatologist Dermatologist { get; set; }
    }
}
