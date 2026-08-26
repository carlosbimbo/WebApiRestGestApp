using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    [Route("api/cenan2026/registrosuplemento")]
    [ApiController]
    public class RegistroSuplementosController : ControllerBase
    {
        private readonly IRegistroSuplementoService _suplementoService;

        public RegistroSuplementosController(IRegistroSuplementoService suplementoService)
        {
            _suplementoService = suplementoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suplementos = await _suplementoService.GetAllAsync();
            return Ok(suplementos);
        }

        // GET: api/RegistroSuplementos/{idSuple}/{idUser}
        [HttpGet("{idSuple}/{idUser}")]
        public async Task<IActionResult> GetByIds(int idSuple, Guid idUser)
        {
            var suplemento = await _suplementoService.GetByIdsAsync(idSuple, idUser);
            if (suplemento == null) return NotFound(new { message = "Suplemento no encontrado" });

            return Ok(suplemento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegistroSuplemento suplemento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _suplementoService.CreateAsync(suplemento);
            return CreatedAtAction(nameof(GetByIds), new { idSuple = created.IdSuple, idUser = created.IdUser }, created);
        }

        // PUT: api/RegistroSuplementos/{idSuple}/{idUser}
        [HttpPut("{idSuple}/{idUser}")]
        public async Task<IActionResult> Update(int idSuple, Guid idUser, [FromBody] RegistroSuplemento suplemento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _suplementoService.UpdateAsync(idSuple, idUser, suplemento);
            if (!updated) return NotFound(new { message = "Suplemento no encontrado para actualizar" });

            return NoContent();
        }

        // DELETE: api/RegistroSuplementos/{idSuple}/{idUser}
        [HttpDelete("{idSuple}/{idUser}")]
        public async Task<IActionResult> Delete(int idSuple, Guid idUser)
        {
            var deleted = await _suplementoService.DeleteAsync(idSuple, idUser);
            if (!deleted) return NotFound(new { message = "Suplemento no encontrado para eliminar" });

            return NoContent();
        }
    }
}