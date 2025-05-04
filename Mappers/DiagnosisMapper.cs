using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Mappers
{
    public class DiagnosisMapper
    {
        public static DiagnosisDto MapToDto(Diagnosis diagnosis)
        {
            return new DiagnosisDto
            {
                Id = diagnosis.Id,
                PatientId = diagnosis.PatientId,
                DermatologistId = diagnosis.DermatologistId,
                LesionId = diagnosis.LesionId,
                DiagnosisDate = diagnosis.DiagnosisDate,
                Description = diagnosis.Description,
            };
        }

        public static Diagnosis MapFromCreateDto(DiagnosisCreateDto dto)
        {
            return new Diagnosis
            {
                PatientId = dto.PatientId,
                DermatologistId = dto.DermatologistId,
                LesionId = dto.LesionId,
                DiagnosisDate = dto.DiagnosisDate,
                Description = dto.Description,
            };
        }

        public static void MapFromUpdateDto(Diagnosis entity, DiagnosisUpdateDto dto)
        {
            entity.PatientId = dto.PatientId;
            entity.DermatologistId = dto.DermatologistId;
            entity.LesionId = dto.LesionId;
            entity.DiagnosisDate = dto.DiagnosisDate;
            entity.Description = dto.Description;
        }
    }
}
