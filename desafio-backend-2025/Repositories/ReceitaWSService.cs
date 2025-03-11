using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace desafio_backend_2025.Repositories
{
    public class ReceitaWSService
    {
        private readonly HttpClient _httpClient;
        private readonly string wsGedApiUrl;

        
        public ReceitaWSService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetNomeEmpresaByCnpj(string cnpj)
        {
            try
            {
                // Formar a URL para a consulta
                 var url = $"https://www.receitaws.com.br/v1/cnpj/{cnpj}";

                // Enviar a requisição GET para a API
                var response = await _httpClient.GetStringAsync(url);

                // Desserializar o JSON retornado
                var dados = JsonConvert.DeserializeObject<ReceitaWSResponse>(response);

                // Verificar se a resposta foi bem-sucedida e retornar o nome da empresa
                if (dados?.Nome != null)
                {
                    return dados.Nome;
                }

                throw new Exception("Nome da empresa não encontrado.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar CNPJ: {ex.Message}");
            }
        }

        // Classe que representa a resposta da API ReceitaWS
        public class ReceitaWSResponse
        {
            public string Nome { get; set; }
            // Adicione outros campos se necessário, como 'CNPJ', 'Fantasia', etc.
        }
    }
}
