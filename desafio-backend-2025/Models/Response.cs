namespace desafio_backend_2025.Models
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Response(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static Response<T> Ok(T data) => new Response<T>(true, null, data);
        public static Response<T> Error(string message) => new Response<T>(false, message, default);
    }
}
