using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Mappers
{
    public class DermatologistMapper
    {
        public static DermatologistDto MapToDto(Dermatologist dermatologist)
        {
            return new DermatologistDto
            {
                Id = dermatologist.Id,
                FirstName = dermatologist.FirstName,
                LastName = dermatologist.LastName,
                LicenseNumber = dermatologist.LicenseNumber,
                Specialization = dermatologist.Specialization,
                Email = dermatologist.Email,
                PhoneNumber = dermatologist.PhoneNumber,
                ETag = Convert.ToBase64String(dermatologist.RowVersion)
            };
        }
    }
}
