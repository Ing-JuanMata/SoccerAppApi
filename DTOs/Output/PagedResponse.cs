namespace API.DTOs.Output
{
    public class PagedResponse<T>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int TotalPages { get; set; }
        public required int TotalCount { get; set; }
        public required List<T> Content { get; set; }
    }
}
