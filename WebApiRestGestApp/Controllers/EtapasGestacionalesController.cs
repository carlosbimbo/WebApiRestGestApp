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
    //[Route("api/cenan2026/etapaestacional")]
    [Route("api/cenan2026")]
    [ApiController]
    public class EtapasGestacionalesController : ControllerBase
    {
        private readonly IEtapaGestacionalService _etapaService;

        public EtapasGestacionalesController(IEtapaGestacionalService etapaService)
        {
            _etapaService = etapaService;
        }
                
        [HttpPost("saveorudparragesta")]
        public async Task<IActionResult> SaveOrupdT05etapagestaArray([FromBody] IEnumerable<EtapaGestacional> etapas)
        {
            try
            {
                if (etapas == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _etapaService.SaveOrUpdEtapaGestaArrayAsync(etapas);

                return Ok(new
                {
                    status = "OK",
                    message = "Success",
                    data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        
        [HttpGet("getall-etapagesta")]
        public async Task<IActionResult> GetAll()
        {
            var etapas = await _etapaService.GetAllAsync();
            return Ok(etapas);
        }

        [HttpGet("getetapagesta-byuser/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var etapa = await _etapaService.GetByIdAsync(id);
            if (etapa == null) return NotFound(new { message = "Etapa gestacional no encontrada" });

            return Ok(etapa);
        }

        [HttpPost("create-etapagesta")]
        public async Task<IActionResult> Create([FromBody] EtapaGestacional etapa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _etapaService.CreateAsync(etapa);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("updetapagesta-byuser/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EtapaGestacional etapa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _etapaService.UpdateAsync(id, etapa);
            if (!updated) return NotFound(new { message = "Etapa gestacional no encontrada para actualizar" });

            return NoContent();
        }

        [HttpDelete("deletapagesta-byuser/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _etapaService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "Etapa gestacional no encontrada para eliminar" });

            return NoContent();
        }
    }
}