using System.ComponentModel.DataAnnotations;

namespace DermatologyApi.DTOs
{
    public class DiagnosisDto
    {
        public required int Id { get; set; }
        public required int PatientId { get; set; }
        public required int DermatologistId { get; set; }
        public required int LesionId { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public required string Description { get; set; }
        public required string ETag { get; set; }
    }

    public class DiagnosisCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DermatologistId { get; set; }

        [Required]
        public int LesionId { get; set; }

        [Required]
        public DateTime DiagnosisDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class DiagnosisUpdateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DermatologistId { get; set; }

        [Required]
        public int LesionId { get; set; }

        [Required]
        public DateTime DiagnosisDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
