using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    [Route("api/cenan2026/agendagestacional")]
    [ApiController]
    public class AgendaGestacionalController : ControllerBase
    {
        private readonly IAgendaGestacionalService _agendaService;

        public AgendaGestacionalController(IAgendaGestacionalService agendaService)
        {
            _agendaService = agendaService;
        }

        // =========================================================
        // NUEVO ENDPOINT MIGRADO DE NODE.JS
        // =========================================================
        [HttpPost("syncallagendagesta")]
        public async Task<IActionResult> SaveOrUpdAgendaGestacionalSync([FromBody] IEnumerable<AgendaGestacional> agendaList)
        {
            try
            {
                if (agendaList == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _agendaService.SaveOrUpdAgendaGestacionalSyncAsync(agendaList);

                return Ok(new
                {
                    status = "OK",
                    message = "Success",
                    data = result
                });
            }
            catch (Exception ex)
            {
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(exactError);
                return StatusCode(500, new { error = "Internal Server Error", details = exactError });
            }
        }

        // =========================================================
        // CRUD ESTÁNDAR
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agendas = await _agendaService.GetAllAsync();
            return Ok(agendas);
        }

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

        [HttpPut("{id}/{nrosem}")]
        public async Task<IActionResult> Update(Guid id, int nrosem, [FromBody] AgendaGestacional agenda)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _agendaService.UpdateAsync(id, nrosem, agenda);
            if (!updated) return NotFound(new { message = "Agenda gestacional no encontrada para actualizar" });

            return NoContent();
        }

        [HttpDelete("{id}/{nrosem}")]
        public async Task<IActionResult> Delete(Guid id, int nrosem)
        {
            var deleted = await _agendaService.DeleteAsync(id, nrosem);
            if (!deleted) return NotFound(new { message = "Agenda gestacional no encontrada para eliminar" });

            return NoContent();
        }
    }
}