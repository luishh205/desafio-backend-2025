using desafio_backend_2025.Models;
using desafio_backend_2025.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using Swashbuckle.AspNetCore.Annotations;

namespace desafio_backend_2025.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Obtém todas os usuarios cadastradas.
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Obtém todos os usuarios cadastrados", Description = "Retorna uma lista de todos as usuarios cadastrados na base de dados.")]
        public async Task<ActionResult<Response<IEnumerable<Usuario>>>> Get()
        {
            try
            {
                var response = await _usuarioRepository.GetAll();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<IEnumerable<Usuario>>.Error($"Erro interno ao buscar usuarios: {ex.Message}"));
            }
        }

        /// <summary>
        /// Obtém uma usuario pelo ID.
        /// </summary>
        /// <param name="id">ID da usuario</param>
        /// <returns>Usuario com o ID especificado</returns>
        [Authorize]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém um usuario pelo ID", Description = "Retorna o usuario com base no ID fornecido.")]
        public async Task<ActionResult<Response<Usuario>>> GetById([FromRoute] int id)
        {
            try
            {
                var response = await _usuarioRepository.GetById(id);
                if (!response.Success)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<Usuario>.Error($"Erro interno ao buscar usuario: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cria um novo usuario.
        /// </summary>
        /// <param name="usuario">Objeto Usuario com os dados a serem salvos</param>
        /// <returns>ID da usuario criada</returns>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Cria uma novo usuario", Description = "Cria um novo usuario utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<int>>> Create([FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<int>.Error("Dados inválidos fornecidos."));
                }
                
                var response = await _usuarioRepository.Create(usuario);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return CreatedAtAction(nameof(GetById), new { id = response.Data }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao criar usuario: {ex.Message}"));
            }
        }

        /// <summary>
        /// Login novo usuario.
        /// </summary>
        /// <param name="usuario">Objeto Usuario com os dados a serem salvos</param>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Realizar Login", Description = "Realiza o login utilizando os dados fornecidos.")]
        public async Task<ActionResult<Response<TokenDTO>>> Login([FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<TokenDTO>.Error("Dados inválidos fornecidos."));
                }

                var response = await _usuarioRepository.GetUsuarioEmail(usuario.email);
                if (!response.Success || response.Data == null)
                {
                    return BadRequest(Response<TokenDTO>.Error("E-mail ou senha inválidos."));
                }

                var usuarioDb = response.Data;
                var token = "";

                if (usuario.password == usuarioDb.password)
                {
                    // Gera o token JWT
                    token = _usuarioRepository.GerarToken(usuario);
                }

                return Ok(Response<TokenDTO>.Ok(new TokenDTO { Token = token }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<TokenDTO>.Error($"Erro interno ao realizar login: {ex.Message}"));
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuario existente.
        /// </summary>
        /// <param name="conta">Objeto Usuario com os dados a serem atualizados</param>
        /// <returns>Status da atualização</returns>
        [Authorize]
        [HttpPut]
        [SwaggerOperation(Summary = "Atualiza os dados de um usuario", Description = "Atualiza as informações de um usurio existente com base no ID.")]
        public async Task<ActionResult<Response<int>>> Update([FromBody] Usuario Usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(Response<int>.Error("Dados inválidos fornecidos."));
                }

                var usuario = await _usuarioRepository.GetById(Usuario.id);
                if (usuario.Data == null)
                {
                    return NotFound(Response<int>.Error($"Usuario com ID {Usuario.id} não encontrado."));
                }

                var response = await _usuarioRepository.Update(Usuario);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao atualizar usuario: {ex.Message}"));
            }
        }

        /// <summary>
        /// Exclui uma usuario pelo ID.
        /// </summary>
        /// <param name="id">ID da usuario</param>
        /// <returns>Status da exclusão</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Exclui uma usuario pelo ID", Description = "Exclui uma usuario com base no ID fornecido.")]
        public async Task<ActionResult<Response<int>>> Delete(int id)
        {
            try
            {
                var empresaExiste = await _usuarioRepository.GetById(id);
                if (empresaExiste.Data == null)
                {
                    return NotFound(Response<int>.Error($"usuario com ID {id} não encontrada."));
                }

                var response = await _usuarioRepository.Delete(id);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, Response<int>.Error($"Erro interno ao deletar usuario: {ex.Message}"));
            }
        }
    }

}
