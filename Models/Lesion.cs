using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DermatologyAPI.Models
{
    public class Lesion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Location { get; set; }

        [Required]
        public DateTime DiscoveryDate { get; set; }

        [Required]
        [StringLength(500)]
        public required string Description {  get; set; }

        public uint Xmin { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public ICollection<Diagnosis>? Diagnoses { get; set; }
    }
}
