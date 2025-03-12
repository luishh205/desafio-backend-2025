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
        [HttpGet]
        [SwaggerOperation(Summary = "Obtém todas as contas cadastradas", Description = "Retorna uma lista de todas as contas cadastradas na base de dados.")]
        public async Task<IEnumerable<Conta>> Get()
        {
            return await _contaRepository.GetAll();
        }

        /// <summary>
        /// Obtém uma conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Conta com o ID especificado</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém uma conta pelo ID", Description = "Retorna os detalhes de uma conta com base no ID fornecido.")]
        public async Task<ActionResult<Conta>> GetById([FromRoute] int id)
        {
            var conta = await _contaRepository.GetById(id);
            if (conta == null)
                return NotFound($"Conta com ID {id} não encontrada.");
            return Ok(conta); 
        }

        /// <summary>
        /// Cria uma nova conta.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem salvos</param>
        /// <returns>ID da conta criada</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova conta", Description = "Cria uma nova conta utilizando os dados fornecidos.")]
        public async Task<IActionResult> Create([FromForm] Conta conta) 
        {
            await _contaRepository.Create(conta);
            return CreatedAtAction(nameof(GetById), new { id = conta.Id }, conta);  
        }

        /// <summary>
        /// Atualiza os dados de uma conta existente.
        /// </summary>
        /// <param name="conta">Objeto Conta com os dados a serem atualizados</param>
        /// <returns>Status da atualização</returns>
        [HttpPut]
        [SwaggerOperation(Summary = "Atualiza os dados de uma conta", Description = "Atualiza as informações de uma conta existente com base no ID.")]
        public async Task<IActionResult> Update([FromForm] Conta conta)
        {
            var empresaExiste = await _contaRepository.VerificarExistenciaEmpresa(conta.Id);
            if (!empresaExiste)
            {
                return NotFound($"Empresa com ID {conta.Id} não encontrada."); 
            }

            await _contaRepository.Update(conta);
            return NoContent();  
        }

        /// <summary>
        /// Exclui uma conta pelo ID.
        /// </summary>
        /// <param name="id">ID da conta</param>
        /// <returns>Status da exclusão</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui uma conta pelo ID", Description = "Exclui uma conta com base no ID fornecido.")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contaRepository.Delete(id);
            return NoContent();  
        }
    }
}
