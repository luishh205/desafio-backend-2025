using desafio_backend_2025.Models;
using desafio_backend_2025.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        /// Obtém saldo da conta cadastrada.
        /// </summary>
        /// <returns>Get Saldo</returns>
        [Authorize]
        [HttpGet("saldo/{id}")]
        [SwaggerOperation(Summary = "Obtém o saldo da conta", Description = "Retorna o saldo da conta na base de dados.")]
        public async Task<ActionResult<Transacao>> GetSaldo([FromRoute] int id)
        {
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
        /// Obtém o extrato da conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Extrado da Conta com o ID especificado</returns>
        [Authorize]
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


        /// <summary>
        /// Realiza um novo deposito.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem salvos</param>
        /// <returns>ID da conta criada</returns>
        [Authorize]
        [HttpPost("deposito")]
        [SwaggerOperation(Summary = "Realizar um novo deposito", Description = "Realizar um deposito utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<bool>>> Depositar( int ContaId, decimal Valor )
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<bool>.Error("Dados inválidos fornecidos."));
                }

                var response = await _transacaoRepository.Depositar(ContaId, Valor);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Response<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<bool>.Error($"Erro interno ao criar um novo deposito: {ex.Message}"));
            }
        }


        /// <summary>
        /// Realiza um novo saque.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem salvos</param>
        /// <returns>ID da conta criada</returns>
        [Authorize]
        [HttpPost("saque")]
        [SwaggerOperation(Summary = "Realizar um novo saque", Description = "Realizar um saque utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<bool>>> Sacar(int ContaId, decimal Valor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<bool>.Error("Dados inválidos fornecidos."));
                }

                var response = await _transacaoRepository.Sacar(ContaId, Valor);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Response<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<bool>.Error($"Erro interno ao criar um novo saque: {ex.Message}"));
            }
        }

        /// <summary>
        /// Realiza uma nova transferencia.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem salvos</param>
        /// <returns>ID da conta criada</returns>
        [Authorize]
        [HttpPost("transferencia")]
        [SwaggerOperation(Summary = "Realizar uma nova transferencia", Description = "Realizar uma nova transferencia utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<bool>>> Transferir(int contaOrigemId, int contaDestinoId, decimal Valor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<bool>.Error("Dados inválidos fornecidos."));
                }
                
                var response = await _transacaoRepository.Transferir(contaOrigemId, contaDestinoId, Valor);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Response<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<bool>.Error($"Erro interno ao criar uma nova transferencia: {ex.Message}"));
            }
        }

    }
}
