using System.ComponentModel.DataAnnotations;

namespace DermatologyApi.DTOs
{
    public class DermatologistDto
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string LicenseNumber { get; set; }
        public required string Specialization { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }

    public class DermatologistCreateDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class DermatologistUpdateDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class DermatologistPatchDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Specialization { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
