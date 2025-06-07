using DermatologyApi.DTOs;
using DermatologyAPI.Models;

namespace DermatologyApi.Services
{
    public interface IConsultationService
    {
        Task<Consultation> GetConsultationEntityByIdAsync(int id);
        Task<PagedResult<ConsultationDto>> GetAllConsultationsAsync(int page, int size);
        Task<ConsultationDto> GetConsultationByIdAsync(int id);
        Task<ConsultationDto> CreateConsultationAsync(ConsultationCreateDto consultationDto);
        Task<ConsultationDto> UpdateConsultationAsync(int id, ConsultationUpdateDto consultationDto, uint expectedXmin);
        Task<bool> DeleteConsultationAsync(int id);
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
