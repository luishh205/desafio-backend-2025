using System.Net.Http;
using System.Threading.Tasks;
using desafio_backend_2025.Models;
using Newtonsoft.Json;


namespace desafio_backend_2025.Repositories
{
    public class ReceitaWSService
    {
        private readonly HttpClient _httpClient;

        public IConfiguration _configuration { get; }

        private readonly string ReceitaWSUrl;
        public ReceitaWSService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            ReceitaWSUrl = _configuration.GetValue<string>("url:ReceitaWSUrl");
        }

        public async Task<ReceitaWSResponse> GetNomeEmpresaByCnpj(string cnpj)
        {
            try
            {
                if (!ValidarCnpj(cnpj))
                {
                    throw new Exception($"CNPJ inválido. Por favor, verifique e tente novamente. {cnpj}");
                }

                cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

                var url = $"{ReceitaWSUrl}cnpj/{cnpj}";

                var response = await _httpClient.GetStringAsync(url);

                if(response == null)
                {
                    throw new Exception("Empresa não encontrado. Por favor, contate o suporte.");
                }

                var dados = JsonConvert.DeserializeObject<ReceitaWSResponse>(response);

                if (dados != null && !string.IsNullOrEmpty(dados.Nome))
                {
                    return dados;
                }

                throw new Exception("Empresa não encontrado. Por favor, contate o suporte.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar CNPJ: {ex.Message}");
            }
        }

        public bool ValidarCnpj(string cnpj)
        {
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            if (cnpj.Length != 14 || !cnpj.All(char.IsDigit))
                return false;

            var digitos = cnpj.Substring(0, 12);
            var dv = cnpj.Substring(12, 2);

            var soma1 = 0;
            var soma2 = 0;
            int[] peso1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] peso2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 12; i++)
                soma1 += int.Parse(cnpj[i].ToString()) * peso1[i];

            var resto1 = soma1 % 11;
            if (resto1 < 2)
                resto1 = 0;
            else
                resto1 = 11 - resto1;

            for (int i = 0; i < 13; i++)
                soma2 += int.Parse(cnpj[i].ToString()) * peso2[i];

            var resto2 = soma2 % 11;
            if (resto2 < 2)
                resto2 = 0;
            else
                resto2 = 11 - resto2;

            return dv == $"{resto1}{resto2}";
        }


        public class ReceitaWSResponse
        {
            public string Nome { get; set; }
            public string Fantasia { get; set; }
            public string Email { get; set; }
            public string Telefone { get; set; }
        }
    }
}
