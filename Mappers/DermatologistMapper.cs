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
            };
        }

        public static Dermatologist MapFromCreateDto(DermatologistCreateDto dto)
        {
            return new Dermatologist
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                LicenseNumber = dto.LicenseNumber,
                Specialization = dto.Specialization,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };
        }

        public static void MapFromUpdateDto(Dermatologist entity, DermatologistUpdateDto dto)
        {
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.LicenseNumber = dto.LicenseNumber;
            entity.Specialization = dto.Specialization;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;
        }
    }
}
