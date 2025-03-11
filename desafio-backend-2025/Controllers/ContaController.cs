using desafio_backend_2025.Models;
using desafio_backend_2025.Repositories;
using Microsoft.AspNetCore.Mvc;
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


        // GET api/contas
        [HttpGet]
        public async Task<IEnumerable<Conta>> Get()
        {
            return await _contaRepository.GetAll();
        }

        // GET api/contas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Conta>> GetById(int id)
        {
            var conta = await _contaRepository.GetById(id);
            if (conta == null)
                return NotFound();
            return Ok(conta); 
        }

        // POST api/contas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Conta conta) 
        {
            await _contaRepository.Create(conta);
            return CreatedAtAction(nameof(GetById), new { id = conta.Id }, conta);  
        }

        // PUT api/contas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Conta conta)  
        {
            if (id != conta.Id)
                return BadRequest();
            await _contaRepository.Update(conta);
            return NoContent();  
        }

        // DELETE api/contas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contaRepository.Delete(id);
            return NoContent();  
        }
    }
}
