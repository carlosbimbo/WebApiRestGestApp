using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiasGestacionController : ControllerBase
    {
        private readonly IDiasGestacionService _diasService;

        public DiasGestacionController(IDiasGestacionService diasService)
        {
            _diasService = diasService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dias = await _diasService.GetAllAsync();
            return Ok(dias);
        }

        // GET: api/DiasGestacion/{idDiasg}/{idUser}
        [HttpGet("{idDiasg}/{idUser}")]
        public async Task<IActionResult> GetByIds(int idDiasg, Guid idUser)
        {
            var dias = await _diasService.GetByIdsAsync(idDiasg, idUser);
            if (dias == null) return NotFound(new { message = "Registro de día no encontrado" });

            return Ok(dias);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiasGestacion dias)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _diasService.CreateAsync(dias);
            return CreatedAtAction(nameof(GetByIds), new { idDiasg = created.IdDiasg, idUser = created.IdUser }, created);
        }

        // PUT: api/DiasGestacion/{idDiasg}/{idUser}
        [HttpPut("{idDiasg}/{idUser}")]
        public async Task<IActionResult> Update(int idDiasg, Guid idUser, [FromBody] DiasGestacion dias)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _diasService.UpdateAsync(idDiasg, idUser, dias);
            if (!updated) return NotFound(new { message = "Registro de día no encontrado para actualizar" });

            return NoContent();
        }

        // DELETE: api/DiasGestacion/{idDiasg}/{idUser}
        [HttpDelete("{idDiasg}/{idUser}")]
        public async Task<IActionResult> Delete(int idDiasg, Guid idUser)
        {
            var deleted = await _diasService.DeleteAsync(idDiasg, idUser);
            if (!deleted) return NotFound(new { message = "Registro de día no encontrado para eliminar" });

            return NoContent();
        }
    }
}