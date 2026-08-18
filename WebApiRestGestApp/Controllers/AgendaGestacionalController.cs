using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize] // 100% Protegido con JWT
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaGestacionalController : ControllerBase
    {
        private readonly IAgendaGestacionalService _agendaService;

        public AgendaGestacionalController(IAgendaGestacionalService agendaService)
        {
            _agendaService = agendaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agendas = await _agendaService.GetAllAsync();
            return Ok(agendas);
        }

        // GET: api/AgendaGestacional/{id}/{nrosem}
        [HttpGet("{id}/{nrosem}")]
        public async Task<IActionResult> GetByIds(Guid id, int nrosem)
        {
            var agenda = await _agendaService.GetByIdsAsync(id, nrosem);
            if (agenda == null) return NotFound(new { message = "Agenda gestacional no encontrada" });

            return Ok(agenda);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AgendaGestacional agenda)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _agendaService.CreateAsync(agenda);
            return CreatedAtAction(nameof(GetByIds), new { id = created.Id, nrosem = created.Nrosem }, created);
        }

        // PUT: api/AgendaGestacional/{id}/{nrosem}
        [HttpPut("{id}/{nrosem}")]
        public async Task<IActionResult> Update(Guid id, int nrosem, [FromBody] AgendaGestacional agenda)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _agendaService.UpdateAsync(id, nrosem, agenda);
            if (!updated) return NotFound(new { message = "Agenda gestacional no encontrada para actualizar" });

            return NoContent();
        }

        // DELETE: api/AgendaGestacional/{id}/{nrosem}
        [HttpDelete("{id}/{nrosem}")]
        public async Task<IActionResult> Delete(Guid id, int nrosem)
        {
            var deleted = await _agendaService.DeleteAsync(id, nrosem);
            if (!deleted) return NotFound(new { message = "Agenda gestacional no encontrada para eliminar" });

            return NoContent();
        }
    }
}