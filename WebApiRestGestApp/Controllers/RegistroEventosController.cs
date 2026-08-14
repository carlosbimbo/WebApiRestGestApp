using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroEventosController : ControllerBase
    {
        private readonly IRegistroEventoService _eventoService;

        public RegistroEventosController(IRegistroEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventos = await _eventoService.GetAllEventosAsync();
            return Ok(eventos);
        }

        [HttpGet("{idEven}/{idUser}")]
        public async Task<IActionResult> GetByIds(int idEven, Guid idUser)
        {
            var evento = await _eventoService.GetEventoByIdsAsync(idEven, idUser);
            if (evento == null) return NotFound(new { message = "Evento no encontrado" });

            return Ok(evento);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegistroEvento evento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _eventoService.CreateEventoAsync(evento);
            return CreatedAtAction(nameof(GetByIds), new { idEven = created.IdEven, idUser = created.IdUser }, created);
        }

        [HttpPut("{idEven}/{idUser}")]
        public async Task<IActionResult> Update(int idEven, Guid idUser, [FromBody] RegistroEvento evento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _eventoService.UpdateEventoAsync(idEven, idUser, evento);
            if (!updated) return NotFound(new { message = "Evento no encontrado para actualizar" });

            return NoContent();
        }

        [HttpDelete("{idEven}/{idUser}")]
        public async Task<IActionResult> Delete(int idEven, Guid idUser)
        {
            var deleted = await _eventoService.DeleteEventoAsync(idEven, idUser);
            if (!deleted) return NotFound(new { message = "Evento no encontrado para eliminar" });

            return NoContent();
        }
    }
}