using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace desafio_backend_2025.Models
{
    public class Transacao

    {
            public int Id { get; set; }

            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "O valor da transação deve ser maior que zero.")]

            public decimal Valor { get; set; }

            [Required]
            public TipoTransacao Tipo { get; set; } 

            [Required]
            [ForeignKey("Conta")]
            public int ContaId { get; set; }

            public DateTime DataTransacao { get; set; }

            public Conta Conta { get; set; }
        
    }
    public enum TipoTransacao
    {
        Saque,
        Deposito,
        Transferencia
    }
}
