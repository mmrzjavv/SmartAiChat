namespace SmartAiChat.Shared.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse Success(string? message = null)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static ApiResponse Failure(string message, List<string>? errors = null)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static ApiResponse Failure(List<string> errors)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Errors = errors,
                Message = "One or more errors occurred"
            };
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static new ApiResponse<T> Failure(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        public static new ApiResponse<T> Failure(List<string> errors)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Errors = errors,
                Message = "One or more errors occurred"
            };
        }

        // Custom Swagger SchemaId to handle generic type conflicts
        public static string GetSchemaId()
        {
            return typeof(ApiResponse<T>).FullName + "_" + typeof(T).Name;
        }
    }
}
