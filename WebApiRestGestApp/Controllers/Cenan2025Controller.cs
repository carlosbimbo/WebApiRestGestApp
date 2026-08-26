using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiRestGestApp.DTOs;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Authorize]
    [Route("api/cenan2026")]
    //[Route("api/[controller]")]
    [ApiController]
    public class Cenan2025Controller : ControllerBase
    {
        private readonly IUserDataService _userDataService;
        private readonly IEmailService _emailService;
        public Cenan2025Controller(IUserDataService userDataService, IEmailService emailService)
        {
            _userDataService = userDataService;
            _emailService = emailService;
        }

        // Endpoint anterior (Requiere JWT)
        [HttpPost("getalldatauser")]
        public async Task<IActionResult> GetAllDataUser([FromBody] UsernameRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userDataService.GetAllDataUserAsync(request.Username);

            if (result == null) return NotFound(new { message = "Usuario no encontrado" });

            return Ok(result);
        }

       
        [AllowAnonymous] 
        [HttpPost("getalldatauserbyemail")]
        public async Task<IActionResult> GetAllDataUserByEmail([FromBody] EmailRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userDataService.GetAllDataUserByEmailAsync(request.Email);

            if (result == null)
            {
                return NotFound(new { message = "Usuario no encontrado con ese email" });
            }

            return Ok(result);
        }

        // NUEVO ENDPOINT: Recuperación de contraseña (USO LIBRE)
        [AllowAnonymous]
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Llamamos a nuestro servicio de email
                await _emailService.SendRecoveryEmailAsync(request.Email, request.NewPassword, request.Nombape);

                // Si todo sale bien, devolvemos tu JSON exacto
                return Ok(new
                {
                    status = "OK",
                    message = "Correo de recuperación enviado exitosamente."
                });
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un JSON con el mensaje exacto de falla (ej. error de Resend)
                return StatusCode(500, new
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }


    }
}