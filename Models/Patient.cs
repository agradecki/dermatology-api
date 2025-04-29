using System.ComponentModel.DataAnnotations;

namespace DermatologyAPI.Models
{
    public class Patient
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
        public required DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(15)]
        public required string PhoneNumber {  get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        public required string Address {  get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
