using System.ComponentModel.DataAnnotations;

namespace desafio_backend_2025.Models
{
    public class Conta
    {
        public int Id { get; set; }

        public required string CNPJ { get; set; }

        [Required]
        public int NumeroConta { get; set; }

        [Required]
        public int Agencia { get; set; }

        public string ImagemDocumento { get; set; }

        public IFormFile Documento { get; set; }

        public string nome { get; set; }
        public string fantasia { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
    }
}
