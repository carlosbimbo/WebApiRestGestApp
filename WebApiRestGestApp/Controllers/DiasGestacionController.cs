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
    [Route("api/cenan2026/diasgestacion")]
    [ApiController]
    public class DiasGestacionController : ControllerBase
    {
        private readonly IDiasGestacionService _diasService;

        public DiasGestacionController(IDiasGestacionService diasService)
        {
            _diasService = diasService;
        }

        // =========================================================
        // NUEVO ENDPOINT MIGRADO DE NODE.JS
        // =========================================================
        [HttpPost("syncalldaysgesta")]
        public async Task<IActionResult> SaveOrUpdT05diasGestacionSync([FromBody] IEnumerable<DiasGestacion> diasList)
        {
            try
            {
                if (diasList == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _diasService.SaveOrUpdT05diasGestacionSyncAsync(diasList);

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

        // =========================================================
        // CRUD ESTÁNDAR
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dias = await _diasService.GetAllAsync();
            return Ok(dias);
        }

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

        [HttpPut("{idDiasg}/{idUser}")]
        public async Task<IActionResult> Update(int idDiasg, Guid idUser, [FromBody] DiasGestacion dias)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _diasService.UpdateAsync(idDiasg, idUser, dias);
            if (!updated) return NotFound(new { message = "Registro de día no encontrado para actualizar" });

            return NoContent();
        }

        [HttpDelete("{idDiasg}/{idUser}")]
        public async Task<IActionResult> Delete(int idDiasg, Guid idUser)
        {
            var deleted = await _diasService.DeleteAsync(idDiasg, idUser);
            if (!deleted) return NotFound(new { message = "Registro de día no encontrado para eliminar" });

            return NoContent();
        }
    }
}