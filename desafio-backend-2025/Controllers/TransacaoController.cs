using desafio_backend_2025.Models;
using desafio_backend_2025.Repositories;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace desafio_backend_2025.Controllers
{
    [ApiController]
    [Route("api/transacao")]
    public class TransacaoController : ControllerBase
    {
        private readonly TransacaoRepository _transacaoRepository;
        public TransacaoController(TransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }
        
        
        /// <summary>
        /// Obtém todas as contas cadastradas.
        /// </summary>
        /// <returns>Lista de contas</returns>
        [HttpGet("saldo/{id}")]
        [SwaggerOperation(Summary = "Obtém o saldo da conta", Description = "Retorna o saldo da conta na base de dados.")]
        public async Task<ActionResult<Transacao>> GetSaldo([FromRoute] int id)
        {
            //return await _transacaoRepository.GetSaldo();
            try
            {
                var response = await _transacaoRepository.GetSaldo(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<Transacao>.Error($"Erro interno ao buscar extrato da conta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtém uma conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Conta com o ID especificado</returns>
        [HttpGet("extrato/{id}")]
        [SwaggerOperation(Summary = "Obtém o extrato da conta pelo ID", Description = "Retorna o extrato da conta com base no ID fornecido.")]
        public async Task<ActionResult<Transacao>> Extrato([FromRoute] int id)
        {
            try
            {
                var response = await _transacaoRepository.GetExtratoContaId(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<Transacao>.Error($"Erro interno ao buscar extrato da conta: {ex.Message}"));
            }
        }

    }
}
