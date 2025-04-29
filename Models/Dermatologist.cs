using System.ComponentModel.DataAnnotations;

namespace DermatologyAPI.Models
{
    public class Dermatologist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LicenseNumber {  get; set; }

        [Required]
        [StringLength(50)]
        public required string Specialization { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public required string Email {  get; set; }

        [Required]
        [StringLength(50)]
        public required string PhoneNumber { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
