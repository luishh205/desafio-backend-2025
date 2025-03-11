namespace desafio_backend_2025.Models
{
    public class Conta
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string NumeroConta { get; set; }
        public string Agencia { get; set; }
        public string ImagemDocumento { get; set; }

        public List<Transacao> Transacoes { get; set; }
    }
}
