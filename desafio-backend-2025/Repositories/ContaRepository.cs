using desafio_backend_2025.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static desafio_backend_2025.Repositories.ReceitaWSService;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace desafio_backend_2025.Repositories
{
    public class ContaRepository
    {
        private readonly DatabaseConnection _db;
        private readonly ReceitaWSService _receitaWSService;

        public ContaRepository(DatabaseConnection db, ReceitaWSService receitaWSService)
        {
            _db = db;
            _receitaWSService = receitaWSService;
        }

        public async Task<Response<IEnumerable<Conta>>> GetAll()
        {
            try
            {
                using var conn = _db.GetConnection();
                var contas = await conn.QueryAsync<Conta>("SELECT * FROM Conta");

                return Response<IEnumerable<Conta>>.Ok(contas);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<Conta>>.Error($"Erro ao buscar todas as contas: {ex.Message}");
            }
        }

        public async Task<Response<Conta>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Response<Conta>.Error("ID deve ser maior que zero.");
                }

                using var conn = _db.GetConnection();

                var conta = await conn.QuerySingleOrDefaultAsync<Conta>(
                    "SELECT * FROM Conta WHERE id = @Id",
                    new { Id = id })
                    .ConfigureAwait(false);

                if (conta == null)
                {
                    return Response<Conta>.Error($"Conta com ID {id} não encontrada.");
                }

                return Response<Conta>.Ok(conta);
            }
            catch (Exception ex)
            {
                return Response<Conta>.Error($"Erro ao buscar conta pelo ID: {id} - {ex.Message}");
            }
        }

        public async Task<Response<int>> Create(Conta conta)
        {
            try
            {
                if (!ValidarConta(conta.NumeroConta, conta.Agencia, out string erro))
                {
                    return Response<int>.Error(erro);
                }

                conta.CNPJ = conta.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");

                var dados = await _receitaWSService.GetNomeEmpresaByCnpj(conta.CNPJ);
                if (dados == null || string.IsNullOrEmpty(dados.Nome))
                {
                    return Response<int>.Error($"Nome da empresa não encontrado para o CNPJ: {conta.CNPJ}");
                }

                conta.nome = dados.Nome;
                conta.fantasia = dados.Fantasia;
                conta.email = dados.Email;
                conta.telefone = dados.Telefone;

                using var conn = _db.GetConnection(); 

                conn.Open();

                using var transaction = conn.BeginTransaction();

                try
                {
                    var result = await conn.ExecuteAsync(
                        "INSERT INTO Conta (nome, fantasia, email, telefone, cnpj, numeroConta, agencia, imagemDocumento) " +
                        "VALUES (@Nome, @fantasia, @email, @telefone, @CNPJ, @NumeroConta, @Agencia, @ImagemDocumento)",
                        conta, transaction);

                    if (conta.Documento != null)
                    {
                        string caminhoDocumento = await SalvarDocumento(conta.Documento);

                        conta.ImagemDocumento = caminhoDocumento;

                        await conn.ExecuteAsync(
                            "UPDATE Conta SET imagemDocumento = @ImagemDocumento WHERE id = @Id",
                            new { ImagemDocumento = caminhoDocumento, Id = conta.Id }, transaction);
                    }

                    transaction.Commit();

                    return Response<int>.Ok(result);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Response<int>.Error($"Erro ao criar conta com CNPJ: {conta.CNPJ} - {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Response<int>.Error($"Erro ao criar conta com CNPJ: {conta.CNPJ} - {ex.Message}");
            }
        }


        public async Task<Response<int>> Update(Conta conta)
        {
            try
            {
                var cnpj = conta.CNPJ.ToString();

                ReceitaWSResponse dados = new ReceitaWSResponse();
                dados = await _receitaWSService.GetNomeEmpresaByCnpj(cnpj);
                if (dados == null || string.IsNullOrEmpty(dados.Nome))
                {
                    return Response<int>.Error($"Empresa não encontrado para o CNPJ: {conta.CNPJ}");
                }
                conta.nome = dados.Nome;
                conta.fantasia = dados.Fantasia;
                conta.email = dados.Email;
                conta.telefone = dados.Telefone;

                using var conn = _db.GetConnection();

                // Buscar caminho do documento atual no banco
                string? caminhoAtual = await conn.ExecuteScalarAsync<string>(
                    "SELECT imagemDocumento FROM Conta WHERE cnpj = @CNPJ", new { conta.CNPJ }
                );

                if (caminhoAtual == null)
                {
                    return Response<int>.Error($"Caminho do documento não encontrado.");
                }

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

                var result = await conn.ExecuteAsync(
                   "UPDATE Conta SET nome = @Nome, fantasia = @fantasia, email = @email, telefone = @telefone, cnpj = @CNPJ, numeroConta = @NumeroConta, agencia = @Agencia, imagemDocumento = @ImagemDocumento WHERE id = @Id",
                   conta);

                return Response<int>.Ok(result);
            }
            catch (Exception ex)
            {
                return Response<int>.Error($"Erro ao atualizar conta com ID: {conta.Id}  {ex.Message}");
            }
        }

        public async Task<Response<bool>> Delete(int id)
        {
            try
            {

                var empresaExiste = await VerificarExistenciaEmpresa(id);
                if (!empresaExiste)
                {
                    return Response<bool>.Error($"Conta com ID {id} não encontrada.");
                }

                using var conn = _db.GetConnection();

                string? caminhoAtual = await conn.ExecuteScalarAsync<string>(
                    "SELECT imagemDocumento FROM Conta WHERE id = @Id", new { Id = id }
                );
                if (!string.IsNullOrEmpty(caminhoAtual))
                {
                    DeletarDocumento(caminhoAtual);
                }

                var result = await conn.ExecuteAsync("DELETE FROM Conta WHERE id = @Id", new { Id = id });

                if (result == 0)
                {
                    return Response<bool>.Error($"Nenhuma conta encontrada com o ID: {id}");
                }

                return Response<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                return Response<bool>.Error($"Erro ao deletar conta com ID: {id} {ex.Message}");
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
                var response = await GetById(id);
                return response.Success && response.Data != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
