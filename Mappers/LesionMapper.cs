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
                ETag = Convert.ToBase64String(lesion.RowVersion)
            };
        }
    }
}
