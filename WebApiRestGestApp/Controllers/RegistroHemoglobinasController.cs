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
    //[Route("api/cenan2026/registrohemoglobina")]
    [Route("api/cenan2026")]
    [ApiController]
    public class RegistroHemoglobinasController : ControllerBase
    {
        private readonly IRegistroHemoglobinaService _hemoglobinaService;

        public RegistroHemoglobinasController(IRegistroHemoglobinaService hemoglobinaService)
        {
            _hemoglobinaService = hemoglobinaService;
        }
               
        [HttpPost("syncallregishemo")]
        public async Task<IActionResult> SaveOrupdRegisHemoSync([FromBody] IEnumerable<RegistroHemoglobina> hemoglobinas)
        {
            try
            {
                if (hemoglobinas == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _hemoglobinaService.SaveOrUpdRegisHemoSyncAsync(hemoglobinas);

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

        
        [HttpGet("getall-regishemo")]
        public async Task<IActionResult> GetAll()
        {
            var hemoglobinas = await _hemoglobinaService.GetAllAsync();
            return Ok(hemoglobinas);
        }

        [HttpGet("getregishemo-byids/{idh}/{idUser}")]
        public async Task<IActionResult> GetByIds(int idh, Guid idUser)
        {
            var hemoglobina = await _hemoglobinaService.GetByIdsAsync(idh, idUser);
            if (hemoglobina == null) return NotFound(new { message = "Registro de hemoglobina no encontrado" });

            return Ok(hemoglobina);
        }

        [HttpPost("create-regishemo")]
        public async Task<IActionResult> Create([FromBody] RegistroHemoglobina hemoglobina)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _hemoglobinaService.CreateAsync(hemoglobina);
            return CreatedAtAction(nameof(GetByIds), new { idh = created.Idh, idUser = created.IdUser }, created);
        }

        [HttpPut("updregishemo-byids/{idh}/{idUser}")]
        public async Task<IActionResult> Update(int idh, Guid idUser, [FromBody] RegistroHemoglobina hemoglobina)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _hemoglobinaService.UpdateAsync(idh, idUser, hemoglobina);
            if (!updated) return NotFound(new { message = "Registro de hemoglobina no encontrado para actualizar" });

            return NoContent();
        }

        [HttpDelete("delregishemo-byids/{idh}/{idUser}")]
        public async Task<IActionResult> Delete(int idh, Guid idUser)
        {
            var deleted = await _hemoglobinaService.DeleteAsync(idh, idUser);
            if (!deleted) return NotFound(new { message = "Registro de hemoglobina no encontrado para eliminar" });

            return NoContent();
        }
    }
}