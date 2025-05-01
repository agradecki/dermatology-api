using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public interface ILesionService
    {
        Task<Lesion> GetLesionEntityByIdAsync(int id);
        Task<IEnumerable<LesionDto>> GetAllLesionsAsync();
        Task<LesionDto> GetLesionByIdAsync(int id);
        Task<LesionDto> CreateLesionAsync(LesionCreateDto lesionDto);
        Task<LesionDto> UpdateLesionAsync(int id, LesionUpdateDto lesionDto, byte[] rowVersion);
        Task<LesionDto> PatchLesionAsync(int id, LesionPatchDto lesionDto);
        Task<bool> DeleteLesionAsync(int id);
    }
}
