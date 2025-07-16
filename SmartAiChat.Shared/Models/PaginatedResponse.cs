namespace SmartAiChat.Shared.Models
{
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        public static PaginatedResponse<T> Create(List<T> items, int totalCount, int page, int pageSize)
        {
            return new PaginatedResponse<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Custom Swagger SchemaId to handle generic type conflicts
        public static string GetSchemaId()
        {
            return typeof(PaginatedResponse<T>).FullName + "_" + typeof(T).Name;
        }
    }
}