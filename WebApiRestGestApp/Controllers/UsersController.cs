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
    //[Route("api/cenan2026/usuario")]
    [Route("api/cenan2026")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
                
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

        
        [HttpGet("getall-users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("getuser-byid/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "Usuario no encontrado" });
            return Ok(user);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("upduser-byid/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _userService.UpdateUserAsync(id, user);
            if (!updated) return NotFound(new { message = "Usuario no encontrado para actualizar" });
            return NoContent();
        }

        [HttpDelete("deluser-byid/{id}")]
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
        [HttpPost("save-userexpotoken")]
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

        
        [AllowAnonymous] // Permite el acceso sin token JWT si así estaba en Node
        [HttpPost("save-userexpotoken-byusername")]
        public async Task<IActionResult> SaveExpoPushTokenByUsername([FromBody] SaveExpoTokenUsernameRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Faltan datos en la petición" });
            }

            try
            {
                var updatedUser = await _userService.UpdateExpoTokenByUsernameAsync(request.Username, request.ExpoPushToken);

                if (updatedUser == null)
                {
                    return BadRequest(new { error = "Usuario no encontrado o no se pudo actualizar" });
                }

                return Ok(new
                {
                    message = "Token actualizado correctamente por username",
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
                return StatusCode(400, new { error = exactError });
            }
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {            
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return StatusCode(204, new { message = "NO_CONTENT" });
            }

            try
            {               
                var user = await _userService.FindUserByNameAsync(request.Username);

                if (user != null)
                {                    
                    return StatusCode(201, new { message = "SUCCESS" });
                }
                else
                {                   
                    return StatusCode(401, new { message = "INVALID_CREDENTIALS" });
                }
            }
            catch (Exception ex)
            {
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine(exactError);
                return StatusCode(500, new { error = "INTERNAL_SERVER_ERROR" });
            }
        }


    }
}