using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Mappers
{
    public class PatientMapper
    {
        public static PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                Address = patient.Address,
                ETag = Convert.ToBase64String(patient.RowVersion)
            };
        }
    }
}
