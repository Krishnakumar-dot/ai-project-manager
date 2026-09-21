namespace AiPmaPlatform.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Response { get; set; }

        public static ApiResponse<T> Success(T response, string message = "Successfully completed transaction.")
            => new() { IsSuccessful = true, Message = message, Response = response };

        public static ApiResponse<T> Fail(string message)
            => new() { IsSuccessful = false, Message = message, Response = default };
    }
}