using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize] 
    [Route("api/cenan2026/etapaestacional")]
    [ApiController]
    public class EtapasGestacionalesController : ControllerBase
    {
        private readonly IEtapaGestacionalService _etapaService;

        public EtapasGestacionalesController(IEtapaGestacionalService etapaService)
        {
            _etapaService = etapaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var etapas = await _etapaService.GetAllAsync();
            return Ok(etapas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var etapa = await _etapaService.GetByIdAsync(id);
            if (etapa == null) return NotFound(new { message = "Etapa gestacional no encontrada" });

            return Ok(etapa);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EtapaGestacional etapa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _etapaService.CreateAsync(etapa);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EtapaGestacional etapa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _etapaService.UpdateAsync(id, etapa);
            if (!updated) return NotFound(new { message = "Etapa gestacional no encontrada para actualizar" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _etapaService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Etapa gestacional no encontrada para eliminar" });

            return NoContent();
        }
    }
}