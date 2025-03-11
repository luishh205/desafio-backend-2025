using desafio_backend_2025.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace desafio_backend_2025.Repositories
{
    public class ContaRepository
    {
        private readonly DatabaseConnection _db;
        private readonly ReceitaWSService _receitaWSService;

        public ContaRepository(DatabaseConnection db)
        {
            _db = db;
        }

       
        public async Task<IEnumerable<Conta>> GetAll()
        {
            using var conn = _db.GetConnection();
            return await conn.QueryAsync<Conta>("SELECT * FROM Conta");
        }

        public async Task<Conta> GetById(int id)
        {
            using var conn = _db.GetConnection();
            return await conn.QueryFirstOrDefaultAsync<Conta>("SELECT * FROM Conta WHERE id = @Id", new { Id = id });
        }

        public async Task<int> Create(Conta conta)
        {
            var nomeEmpresa = await _receitaWSService.GetNomeEmpresaByCnpj(conta.CNPJ);
            if (nomeEmpresa == null)
            {
                throw new Exception("Nome da empresa não encontrado para o CNPJ fornecido.");
            }


            using var conn = _db.GetConnection();
            return await conn.ExecuteAsync(
                "INSERT INTO Conta (nome, cnpj, numero_conta, agencia, imagem_documento) VALUES (@Nome, @CNPJ, @NumeroConta, @Agencia, @ImagemDocumento)",
                conta
            );
        }

        public async Task<int> Update(Conta conta)
        {
            using var conn = _db.GetConnection();
            return await conn.ExecuteAsync(
                "UPDATE Conta SET nome = @Nome, cnpj = @CNPJ, numero_conta = @NumeroConta, agencia = @Agencia, imagem_documento = @ImagemDocumento WHERE id = @Id",
                conta
            );
        }

        public async Task<int> Delete(int id)
        {
            using var conn = _db.GetConnection();
            return await conn.ExecuteAsync("DELETE FROM Conta WHERE id = @Id", new { Id = id });
        }
    }
}
