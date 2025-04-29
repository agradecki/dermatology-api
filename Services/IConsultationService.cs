using DermatologyApi.DTOs;

namespace DermatologyApi.Services
{
    public interface IConsultationService
    {
        Task<PagedResult<ConsultationDto>> GetAllConsultationsAsync(int page, int size);
        Task<ConsultationDto> GetConsultationByIdAsync(int id);
        Task<ConsultationDto> CreateConsultationAsync(ConsultationCreateDto consultationDto);
        Task<ConsultationDto> UpdateConsultationAsync(int id, ConsultationUpdateDto consultationDto, string etag);
        Task DeleteConsultationAsync(int id);
        Task TransferConsultationsAsync(TransferRequest[] transfers);
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class TransferRequest
    {
        public int ConsultationId { get; set; }
        public DateTime NewDateTime { get; set; }
    }
}
