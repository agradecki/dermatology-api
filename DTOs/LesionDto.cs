using System.ComponentModel.DataAnnotations;

namespace DermatologyApi.DTOs
{
    public class LesionDto
    {
        public required int Id { get; set; }
        public required int PatientId { get; set; }
        public required string Location { get; set; }
        public required DateTime DiscoveryDate { get; set; }
        public required string Description { get; set; }
        public required string ETag { get; set; }
    }

    public class LesionCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime DiscoveryDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

    }

    public class LesionUpdateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime DiscoveryDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

    }

    public class LesionPatchDto
    {
        public int? PatientId { get; set; }
        public string? Location { get; set; }
        public DateTime? DiscoveryDate { get; set; }
        public string? Description { get; set; }
    }
}
