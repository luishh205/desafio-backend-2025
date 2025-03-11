namespace desafio_backend_2025.Models
{
    public class Transacao

    {
            public int Id { get; set; }
            public decimal Valor { get; set; }
            public string Tipo { get; set; }  // "saque", "deposito", "transferencia"
            public int ContaId { get; set; }
            public DateTime DataTransacao { get; set; }

            public Conta Conta { get; set; }
        
    }
}
