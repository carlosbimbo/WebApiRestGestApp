using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize] // Protegido con JWT
    [Route("api/cenan2026/registrohemoglobina")]
    [ApiController]
    public class RegistroHemoglobinasController : ControllerBase
    {
        private readonly IRegistroHemoglobinaService _hemoglobinaService;

        public RegistroHemoglobinasController(IRegistroHemoglobinaService hemoglobinaService)
        {
            _hemoglobinaService = hemoglobinaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var hemoglobinas = await _hemoglobinaService.GetAllAsync();
            return Ok(hemoglobinas);
        }

        // GET: api/RegistroHemoglobinas/{idh}/{idUser}
        [HttpGet("{idh}/{idUser}")]
        public async Task<IActionResult> GetByIds(int idh, Guid idUser)
        {
            var hemoglobina = await _hemoglobinaService.GetByIdsAsync(idh, idUser);
            if (hemoglobina == null) return NotFound(new { message = "Registro de hemoglobina no encontrado" });

            return Ok(hemoglobina);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegistroHemoglobina hemoglobina)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _hemoglobinaService.CreateAsync(hemoglobina);
            return CreatedAtAction(nameof(GetByIds), new { idh = created.Idh, idUser = created.IdUser }, created);
        }

        // PUT: api/RegistroHemoglobinas/{idh}/{idUser}
        [HttpPut("{idh}/{idUser}")]
        public async Task<IActionResult> Update(int idh, Guid idUser, [FromBody] RegistroHemoglobina hemoglobina)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _hemoglobinaService.UpdateAsync(idh, idUser, hemoglobina);
            if (!updated) return NotFound(new { message = "Registro de hemoglobina no encontrado para actualizar" });

            return NoContent();
        }

        // DELETE: api/RegistroHemoglobinas/{idh}/{idUser}
        [HttpDelete("{idh}/{idUser}")]
        public async Task<IActionResult> Delete(int idh, Guid idUser)
        {
            var deleted = await _hemoglobinaService.DeleteAsync(idh, idUser);
            if (!deleted) return NotFound(new { message = "Registro de hemoglobina no encontrado para eliminar" });

            return NoContent();
        }
    }
}