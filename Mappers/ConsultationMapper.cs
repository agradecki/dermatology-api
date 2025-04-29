using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Mappers
{
    public class ConsultationMapper
    {
        public static ConsultationDto MapToDto(Consultation consultation)
        {
            return new ConsultationDto
            {
                Id = consultation.Id,
                PatientId = consultation.PatientId,
                DermatologistId = consultation.DermatologistId,
                ConsultationDate = consultation.ConsultationDate,
                Description = consultation.Description,
                PatientName = consultation.Patient?.FirstName,
                DermatologistName = consultation.Dermatologist?.FirstName,
                ETag = Convert.ToBase64String(consultation.RowVersion),
            };
        }
        public static Consultation MapFromCreateDto(ConsultationCreateDto dto)
        {
            return new Consultation
            {
                PatientId = dto.PatientId,
                DermatologistId = dto.DermatologistId,
                ConsultationDate = dto.ConsultationDate,
                Description = dto.Description
            };
        }

        public static void MapFromUpdateDto(ConsultationUpdateDto dto, Consultation entity)
        {
            entity.PatientId = dto.PatientId;
            entity.DermatologistId = dto.DermatologistId;
            entity.ConsultationDate = dto.ConsultationDate;
            entity.Description = dto.Description;
        }
    }
}
