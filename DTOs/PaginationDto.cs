namespace DermatologyApi.DTOs
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class PaginatedResponseDto<T>
    {
        public required List<T> Items { get; set; }
        public int TotalItems {  get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize {  get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
