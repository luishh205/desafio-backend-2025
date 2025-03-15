namespace desafio_backend_2025.Models
{
    public class Usuario
    {
        public int id { get; set; }
        public string email { get; set; } 
        public string password { get; set; } 
    }
    public class TokenDTO
    {
        public string Token { get; set; } = string.Empty;
    }
}
