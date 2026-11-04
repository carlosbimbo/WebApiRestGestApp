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
    [Route("api/cenan2026/registrosuplemento")]
    [ApiController]
    public class RegistroSuplementosController : ControllerBase
    {
        private readonly IRegistroSuplementoService _suplementoService;

        public RegistroSuplementosController(IRegistroSuplementoService suplementoService)
        {
            _suplementoService = suplementoService;
        }

        // =========================================================
        // NUEVO ENDPOINT MIGRADO DE NODE.JS
        // =========================================================

        [HttpPost("syncallsuple")]
        public async Task<IActionResult> SaveOrupdSupleSync([FromBody] IEnumerable<RegistroSuplemento> suplementos)
        {
            try
            {
                if (suplementos == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _suplementoService.SaveOrUpdSupleSyncAsync(suplementos);

                return Ok(new
                {
                    status = "OK",
                    message = "Sincronización completada con éxito",
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
            var suplementos = await _suplementoService.GetAllAsync();
            return Ok(suplementos);
        }

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

        [HttpPut("{idSuple}/{idUser}")]
        public async Task<IActionResult> Update(int idSuple, Guid idUser, [FromBody] RegistroSuplemento suplemento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _suplementoService.UpdateAsync(idSuple, idUser, suplemento);
            if (!updated) return NotFound(new { message = "Suplemento no encontrado para actualizar" });

            return NoContent();
        }

        [HttpDelete("{idSuple}/{idUser}")]
        public async Task<IActionResult> Delete(int idSuple, Guid idUser)
        {
            var deleted = await _suplementoService.DeleteAsync(idSuple, idUser);
            if (!deleted) return NotFound(new { message = "Suplemento no encontrado para eliminar" });

            return NoContent();
        }
    }
}