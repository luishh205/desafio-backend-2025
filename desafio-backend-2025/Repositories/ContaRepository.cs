using desafio_backend_2025.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static desafio_backend_2025.Repositories.ReceitaWSService;
using System.Text;

namespace desafio_backend_2025.Repositories
{
    public class ContaRepository
    {
        private readonly DatabaseConnection _db;
        private readonly ReceitaWSService _receitaWSService;
        private readonly ILogger<ContaRepository> _logger;

        public ContaRepository(DatabaseConnection db, ReceitaWSService receitaWSService, ILogger<ContaRepository> logger)
        {
            _db = db;
            _receitaWSService = receitaWSService;
            _logger = logger;
        }

        public async Task<IEnumerable<Conta>> GetAll()
        {
            try
            {
                using var conn = _db.GetConnection();
                return await conn.QueryAsync<Conta>("SELECT * FROM Conta");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar todas as contas. {ex.Message}");
            }
        }

        public async Task<Conta> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception($"ID deve ser maior que zero. {id}");
                }

                using var conn = _db.GetConnection();

                var conta = await conn.QuerySingleOrDefaultAsync<Conta>(
                    "SELECT * FROM Conta WHERE id = @Id",
                    new { Id = id })
                    .ConfigureAwait(false);

                return conta;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar conta pelo ID: {id} {ex.Message}");
            }
        }

        public async Task<int> Create(Conta conta)
        {
            try
            {
                string erro;
                if (!ValidarConta(conta.NumeroConta, conta.Agencia, out erro))
                {
                    throw new Exception(erro);
                }

                conta.CNPJ = conta.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");

                ReceitaWSResponse dados = new ReceitaWSResponse();
                dados = await _receitaWSService.GetNomeEmpresaByCnpj(conta.CNPJ);
                if (dados == null || string.IsNullOrEmpty(dados.Nome))
                {
                    throw new Exception($"Nome da empresa não encontrado para o CNPJ: {conta.CNPJ}");
                }
                conta.nome = dados.Nome;
                conta.fantasia = dados.Fantasia;
                conta.email = dados.Email;
                conta.telefone = dados.Telefone;

                // Salva o documento e armazena o caminho no banco
                if (conta.Documento != null)
                {
                    conta.ImagemDocumento = await SalvarDocumento(conta.Documento);
                }

                using var conn = _db.GetConnection();
                return await conn.ExecuteAsync(
                    "INSERT INTO Conta (nome, fantasia, email, telefone, cnpj, numeroConta, agencia, imagemDocumento) " +
                    "VALUES (@Nome, @fantasia, @email, @telefone, @CNPJ, @NumeroConta, @Agencia, @ImagemDocumento)",
                    conta
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar conta com CNPJ: {conta.CNPJ} {ex.Message}");
            }
        }

        public async Task<int> Update(Conta conta)
        {
            try
            {
                var cnpj = conta.CNPJ.ToString();

                ReceitaWSResponse dados = new ReceitaWSResponse();
                dados = await _receitaWSService.GetNomeEmpresaByCnpj(cnpj);
                if (dados == null || string.IsNullOrEmpty(dados.Nome))
                {
                    throw new Exception($"Empresa não encontrado para o CNPJ: {conta.CNPJ}");
                }
                conta.nome = dados.Nome;
                conta.fantasia = dados.Fantasia;
                conta.email = dados.Email;
                conta.telefone = dados.Telefone;

                using var conn = _db.GetConnection();

                // Buscar caminho do documento atual no banco
                string caminhoAtual = await conn.ExecuteScalarAsync<string>(
                    "SELECT imagemDocumento FROM Conta WHERE cnpj = @CNPJ", new { conta.CNPJ }
                );

                if (conta.Documento != null)
                {
                    string novoCaminho = await SalvarDocumento(conta.Documento);

                    if (!string.Equals(caminhoAtual, novoCaminho, StringComparison.OrdinalIgnoreCase))
                    {
                        DeletarDocumento(caminhoAtual);
                        conta.ImagemDocumento = novoCaminho;
                    }
                    else
                    {
                        conta.ImagemDocumento = caminhoAtual;
                    }
                }
                else
                {
                    conta.ImagemDocumento = caminhoAtual;
                }

                return await conn.ExecuteAsync(
                    "UPDATE Conta SET nome = @Nome,fantasia = @fantasia,email = @email,telefone = @telefone, cnpj = @CNPJ, numeroConta = @NumeroConta, agencia = @Agencia, imagemDocumento = @ImagemDocumento WHERE id = @Id",
                    conta
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar conta com ID: {conta.Id}  {ex.Message}");
            }
        }

        public async Task<int> Delete(int id)
        {
            try
            {
                using var conn = _db.GetConnection();
                return await conn.ExecuteAsync("DELETE FROM Conta WHERE id = @Id", new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar conta com CNPJ: {id} {ex.Message}");
            }
        }

        private async Task<string> SalvarDocumento(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Arquivo inválido!");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{fileName}"; //caminho da images
        }

        private void DeletarDocumento(string caminhoRelativo)
        {
            if (string.IsNullOrEmpty(caminhoRelativo)) return;

            string caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoRelativo.TrimStart('/'));

            if (File.Exists(caminhoFisico))
            {
                File.Delete(caminhoFisico);
            }
        }



        public static bool ValidarConta(int numeroConta, int agencia, out string mensagemErro)
        {
            if (numeroConta <= 0)
            {
                mensagemErro = "O número da conta deve ser maior que zero.";
                return false;
            }

            if (agencia <= 0)
            {
                mensagemErro = "A agência deve ser maior que zero.";
                return false;
            }

            mensagemErro = "";
            return true;
        }


        public async Task<bool> VerificarExistenciaEmpresa(int id)
        {
            try
            {
                var empresa = await GetById(id);
                return empresa != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar se existe conta com CNPJ: {id} {ex.Message}");
            }
        }
    }
}
