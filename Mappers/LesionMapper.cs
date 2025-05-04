using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Mappers
{
    public class LesionMapper
    {
        public static LesionDto MapToDto(Lesion lesion)
        {
            return new LesionDto
            {
                Id = lesion.Id,
                PatientId = lesion.PatientId,
                Location = lesion.Location,
                DiscoveryDate = lesion.DiscoveryDate,
                Description = lesion.Description,
            };
        }
        public static Lesion MapFromCreateDto(LesionCreateDto dto)
        {
            return new Lesion
            {
                PatientId = dto.PatientId,
                Location = dto.Location,
                DiscoveryDate = dto.DiscoveryDate,
                Description = dto.Description,
            };
        }

        public static void MapFromUpdateDto(LesionUpdateDto dto, Lesion entity)
        {
            entity.PatientId = dto.PatientId;
            entity.Location = dto.Location;
            entity.DiscoveryDate = dto.DiscoveryDate;
            entity.Description = dto.Description;
        }
    }
}
