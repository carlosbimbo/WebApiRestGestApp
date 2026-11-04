using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    //[Route("api/cenan2026/valoracionruletamenu")]
    [Route("api/cenan2026")]
    [ApiController]
    public class ValoracionRuletaMenuController : ControllerBase
    {
        private readonly IValoracionRuletaMenuService _valoracionService;
        private readonly IWebSocketConnectionManager _wsManager; 

        public ValoracionRuletaMenuController(
            IValoracionRuletaMenuService valoracionService,
            IWebSocketConnectionManager wsManager)
        {
            _valoracionService = valoracionService;
            _wsManager = wsManager;
        }
                          

        [HttpGet("likesmenu/{idmenu}")]
        public async Task<IActionResult> GetLikesByMenu(int idmenu)
        {
            try
            {
                var totalLikes = await _valoracionService.CountLikesByMenuAsync(idmenu);

                return Ok(new
                {
                    status = "OK",
                    message = "Success",
                    data = new { totalLikes }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpPost("savelikemenu")]
        public async Task<IActionResult> SaveOrUpdValoracionMenu([FromBody] ValoracionRuletaMenu valoracion)
        {
            try
            {
                if (string.IsNullOrEmpty(valoracion.IdUser) || valoracion.IdMenu <= 0)
                {
                    return BadRequest(new { error = "Los campos 'iduser' e 'idmenu' son obligatorios" });
                }
               
                var totalLikes = await _valoracionService.SaveOrUpdateValoracionAsync(valoracion);
                
                var messageObj = new
                {
                    type = "LIKE_UPDATE",
                    idmenu = valoracion.IdMenu,
                    totalLikes = totalLikes
                };
                string jsonMessage = JsonSerializer.Serialize(messageObj);
               
                await _wsManager.BroadcastMessageAsync(jsonMessage);

                return Ok(new
                {
                    status = "OK",
                    message = "Success",
                    data = new { totalLikes }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }


        [HttpGet("getall-valmenu")]
        public async Task<IActionResult> GetAll()
        {
            var valoraciones = await _valoracionService.GetAllAsync();
            return Ok(valoraciones);
        }

        [HttpGet("getvalmenu-byids/{iduser}/{idmenu}")]
        public async Task<IActionResult> GetById(string iduser, int idmenu)
        {
            var valoracion = await _valoracionService.GetByIdAsync(iduser, idmenu);
            if (valoracion == null) return NotFound(new { message = "Valoración de menú no encontrada" });

            return Ok(valoracion);
        }

        [HttpPost("create-valmenu")]
        public async Task<IActionResult> Create([FromBody] ValoracionRuletaMenu valoracion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _valoracionService.CreateAsync(valoracion);
            return CreatedAtAction(nameof(GetById), new { iduser = created.IdUser, idmenu = created.IdMenu }, created);
        }

        [HttpPut("updvalmenu-byids/{iduser}/{idmenu}")]
        public async Task<IActionResult> Update(string iduser, int idmenu, [FromBody] ValoracionRuletaMenu valoracion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _valoracionService.UpdateAsync(iduser, idmenu, valoracion);
            if (!updated) return NotFound(new { message = "Valoración no encontrada para actualizar" });

            return NoContent();
        }

        [HttpDelete("delvalmenu-byids/{iduser}/{idmenu}")]
        public async Task<IActionResult> Delete(string iduser, int idmenu)
        {
            var deleted = await _valoracionService.DeleteAsync(iduser, idmenu);
            if (!deleted) return NotFound(new { message = "Valoración no encontrada para eliminar" });

            return NoContent();
        }
    }
}