using Dapper;
using desafio_backend_2025.Models;
using Microsoft.AspNetCore.Mvc;

namespace desafio_backend_2025.Repositories
{
    public class TransacaoRepository
    {
        private readonly DatabaseConnection _db;
        private readonly ContaRepository _ContaRepository;
        public TransacaoRepository(DatabaseConnection db,ContaRepository contaRepository)
        {
            _ContaRepository = contaRepository;
            _db = db;
        }

        internal async Task<Response<IEnumerable<Transacao>>> GetExtratoContaId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Response<IEnumerable<Transacao>>.Error("ID deve ser maior que zero.");
                }

                var contaResponse = await _ContaRepository.GetById(id);
                if (!contaResponse.Success || contaResponse.Data == null)
                {
                    return Response<IEnumerable<Transacao>>.Error($"Conta com ID {id} não encontrada.");
                }

                using var conn = _db.GetConnection();

                var extrato = await conn.QueryAsync<Transacao>(
                "SELECT * FROM Transacoes WHERE contaId = @Id ORDER BY dataTransacao DESC",
                new { Id = id });

                foreach (var transacao in extrato)
                {
                    transacao.Conta = await conn.QueryFirstOrDefaultAsync<Conta>(
                        "SELECT * FROM Conta WHERE id = @ContaId",
                        new { ContaId = transacao.ContaId });
                }

                if (extrato == null || !extrato.Any())
                {
                    return Response<IEnumerable<Transacao>>.Ok(new List<Transacao>());
                }

                return Response<IEnumerable<Transacao>>.Ok(extrato);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<Transacao>>.Error($"Erro ao buscar extrato da conta com ID {id}: {ex.Message}");
            }
        }

        internal async Task<Response<IEnumerable<Transacao>>> GetSaldo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Response<IEnumerable<Transacao>>.Error("ID deve ser maior que zero.");
                }

                var contaResponse = await _ContaRepository.GetById(id);
                if (!contaResponse.Success || contaResponse.Data == null)
                {
                    return Response<IEnumerable<Transacao>>.Error($"Conta com ID {id} não encontrada.");
                }

                using var conn = _db.GetConnection();

                var saldoConta = await conn.QueryAsync<Transacao>(
                @"SELECT 
                    SUM(CASE 
                            WHEN tipo = 'deposito' THEN valor
                            WHEN tipo = 'saque' THEN -valor
                            WHEN tipo = 'transferencia' THEN -valor
                            ELSE 0
                        END) AS saldo,
                        contaId
                FROM crud.transacoes
                WHERE contaId = @Id ",
                new { Id = id });
                
                foreach (var transacao in saldoConta)
                {
                    transacao.Conta = await conn.QueryFirstOrDefaultAsync<Conta>(
                        "SELECT * FROM Conta WHERE id = @ContaId",
                        new { ContaId = transacao.ContaId });
                }

                if (saldoConta == null || !saldoConta.Any())
                {
                    return Response<IEnumerable<Transacao>>.Ok(new List<Transacao>());
                }

                return Response<IEnumerable<Transacao>>.Ok(saldoConta);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<Transacao>>.Error($"Erro ao buscar extrato da conta com ID {id}: {ex.Message}");
            }
        }
    }
}
