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
    }
}
