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
    [Route("api/contas")]
    public class ContaController : ControllerBase 
    {
        private readonly ContaRepository _contaRepository;

        public ContaController(ContaRepository contaRepository)
        {
            _contaRepository = contaRepository;
        }

        /// <summary>
        /// Obtém todas as contas cadastradas.
        /// </summary>
        /// <returns>Lista de contas</returns>
        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Obtém todas as contas cadastradas", Description = "Retorna uma lista de todas as contas cadastradas na base de dados.")]
        public async Task<ActionResult<Response<IEnumerable<Conta>>>> Get()
        {
            try
            {
                var response = await _contaRepository.GetAll();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<IEnumerable<Conta>>.Error($"Erro interno ao buscar contas: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtém uma conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Conta com o ID especificado</returns>
        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém uma conta pelo ID", Description = "Retorna os detalhes de uma conta com base no ID fornecido.")]
        public async Task<ActionResult<Response<Conta>>> GetById([FromRoute] int id)
        {
            try
            {
                var response = await _contaRepository.GetById(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<Conta>.Error($"Erro interno ao buscar conta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cria uma nova conta.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem salvos</param>
        /// <returns>ID da conta criada</returns>
        [Authorize]
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova conta", Description = "Cria uma nova conta utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<int>>> Create([FromForm] Conta conta) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<int>.Error("Dados inválidos fornecidos."));
                }

                var response = await _contaRepository.Create(conta);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao criar conta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Atualiza os dados de uma conta existente.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem atualizados</param>
        /// <returns>Status da atualização</returns>
        [Authorize]
        [HttpPut]
        [SwaggerOperation(Summary = "Atualiza os dados de uma conta", Description = "Atualiza as informações de uma conta existente com base no ID.")]
        public async Task<ActionResult<Response<int>>> Update([FromForm] Conta conta)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<int>.Error("Dados inválidos fornecidos."));
                }

                var empresaExiste = await _contaRepository.VerificarExistenciaEmpresa(conta.Id);
                if (!empresaExiste)
                {
                    return NotFound(Response<int>.Error($"Conta com ID {conta.Id} não encontrada."));
                }

                var response = await _contaRepository.Update(conta);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao atualizar conta: {ex.Message}"));
            }
        }

        /// <summary>
        /// Exclui uma conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Status da exclusão</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui uma conta pelo ID", Description = "Exclui uma conta com base no ID fornecido.")]
        public async Task<ActionResult<Response<int>>> Delete(int id)
        {
            try
            {
                var empresaExiste = await _contaRepository.VerificarExistenciaEmpresa(id);
                if (!empresaExiste)
                {
                    return NotFound(Response<int>.Error($"Conta com ID {id} não encontrada."));
                }

                var response = await _contaRepository.Delete(id);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao deletar conta: {ex.Message}"));
            }
        }
    }
}
