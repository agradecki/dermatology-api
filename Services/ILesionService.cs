using DermatologyApi.DTOs;

namespace DermatologyApi.Services
{
    public interface ILesionService
    {
        Task<IEnumerable<LesionDto>> GetAllLesionsAsync();
        Task<LesionDto> GetLesionByIdAsync(int id);
        Task<LesionDto> CreateLesionAsync(LesionCreateDto lesionDto);
        Task<LesionDto> UpdateLesionAsync(int id, LesionUpdateDto lesionDto, byte[] rowVersion);
        Task<LesionDto> PatchLesionAsync(int id, LesionPatchDto lesionDto, byte[] rowVersion);
        Task<bool> DeleteLesionAsync(int id);
    }
}
