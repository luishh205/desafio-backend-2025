namespace desafio_backend_2025.Models
{
    public class Response<T>
    { 
        public bool Success { get; set; }    
        public string Message { get; set; }  
        public T Data { get; set; }          
        public List<string> Errors { get; set; } 
        public int StatusCode { get; set; } 

        // Construtor de sucesso
        public Response(bool success, string message, T data, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = new List<string>();
            StatusCode = statusCode;
        }

        // Construtor de erro
        public Response(bool success, string message, List<string> errors, int statusCode)
        {
            Success = success;
            Message = message;
            Data = default;
            Errors = errors ?? new List<string>();
            StatusCode = statusCode;
        }
    }
}
