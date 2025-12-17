namespace HotelManager.API.Responses
{
    public class PagedResponse<T> : ApiResponse<T>
    {
        public PaginationMetadata Pagination { get; set; }

        public PagedResponse(string message, T data, PaginationMetadata pagination)
            : base(message, data)
        {
            Pagination = pagination;
        }
    }

    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }

}
