using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiRestGestApp.DTOs;
using WebApiRestGestApp.Models;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    [Route("api/cenan2026/usuario")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // =========================================================
        // NUEVO ENDPOINT MIGRADO DE NODE.JS
        // =========================================================
        [HttpPost("syncalluser")]
        public async Task<IActionResult> SaveOrupdUserSync([FromBody] IEnumerable<User> usersList)
        {
            try
            {
                if (usersList == null)
                {
                    return BadRequest(new { error = "El payload no puede estar vacío" });
                }

                var result = await _userService.SaveOrUpdUserSyncAsync(usersList);

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
        // CRUD Y ENDPOINTS EXISTENTES
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _userService.UpdateUserAsync(id, user);
            if (!updated) return NotFound(new { message = "Usuario no encontrado para actualizar" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted) return NotFound(new { message = "Usuario no encontrado para eliminar" });
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("newuserfirstsign")]
        public async Task<IActionResult> SaveOrUpdate([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _userService.SaveOrUpdateAsync(user);
                return Ok(new { message = "SUCCESS", data = result });
            }
            catch (Exception ex)
            {
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { message = "Error interno del servidor", details = exactError });
            }
        }

        [AllowAnonymous]
        [HttpPost("api/cenan2026/save-userexpotoken")]
        public async Task<IActionResult> SaveUserExpoToken([FromBody] SaveExpoTokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(new { error = "Faltan datos en la petición" });
            try
            {
                if (!Guid.TryParse(request.UserId, out Guid parsedUserId))
                {
                    return BadRequest(new { error = $"invalid input syntax for type uuid: \"{request.UserId}\"" });
                }

                var updatedUser = await _userService.UpdateExpoTokenAsync(parsedUserId, request.ExpoPushToken);

                if (updatedUser == null) return NotFound(new { error = "Usuario no encontrado en la base de datos" });

                return Ok(new
                {
                    message = "Token actualizado",
                    user = new
                    {
                        id = updatedUser.Id,
                        username = updatedUser.Username,
                        expopushtoken = updatedUser.ExpoPushToken,
                        updated_at = updatedUser.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = exactError });
            }
        }
    }
}