using System.ComponentModel.DataAnnotations;

namespace DermatologyApi.DTOs
{
    public class ConsultationDto
    {
        public required int Id { get; set; }
        public required int PatientId { get; set; }
        public required int DermatologistId { get; set; }
        public required DateTime ConsultationDate { get; set; }
        public string? Description { get; set; }
        public string? PatientName { get; set; }
        public string? DermatologistName { get; set; }
        public string? ETag { get; set; }
    }

    public class ConsultationCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DermatologistId { get; set; }

        [Required]
        public DateTime ConsultationDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class ConsultationUpdateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DermatologistId { get; set; }

        [Required]
        public DateTime ConsultationDate { get; set; }

        public string? Description { get; set; }
    }
}
