using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiRestGestApp.DTOs;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [Route("api/cenan2026")] 
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var token = await _authService.AuthenticateAsync(request);

            if (token == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            // Retornamos exactamente la estructura esperada
            return Ok(new
            {
                message = "SUCCESS",
                token = token
            });
        }
    }
}