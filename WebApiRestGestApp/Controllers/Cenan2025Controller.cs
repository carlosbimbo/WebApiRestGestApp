using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; 
using System;
using System.Collections.Generic;
using System.IO; 
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
        private readonly IConfiguration _configuration;

        public Cenan2025Controller(IUserDataService userDataService, IEmailService emailService, IConfiguration configuration)
        {
            _userDataService = userDataService;
            _emailService = emailService;
            _configuration = configuration;
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
                
        [AllowAnonymous]
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {                
                await _emailService.SendRecoveryEmailAsync(request.Email, request.NewPassword, request.Nombape);
                                
                return Ok(new
                {
                    status = "OK",
                    message = "Correo de recuperación enviado exitosamente."
                });
            }
            catch (Exception ex)
            {                
                return StatusCode(500, new
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        
        [AllowAnonymous]
        [HttpGet("fotos-usuario/{username}")]
        public async Task<IActionResult> GetFotosPorUsuario(string username)
        {
            try
            {
                var result = await _userDataService.GetFotosPorUsuarioAsync(username);
                                
                if (result == null)
                {
                    return NotFound(new { error = "Usuario no encontrado" });
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { error = exactError });
            }
        }
                
        [AllowAnonymous]
        [HttpPost("upload-fotos")]
        public async Task<IActionResult> UploadFotos([FromForm] List<IFormFile> fotos)
        {            
            if (fotos == null || fotos.Count == 0)
            {
                return BadRequest(new
                {
                    ok = false,
                    message = "No se enviaron archivos válidos"
                });
            }

            try
            {                
                var uploadPath = _configuration["Uploads:FotosPath"];
                                
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var savedFiles = new List<string>();
                                
                foreach (var formFile in fotos)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = formFile.FileName;
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Guardar el archivo físicamente en el disco duro del servidor
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        // Añadimos el nombre al arreglo de respuesta
                        savedFiles.Add(fileName);
                    }
                }

                return Ok(new
                {
                    ok = true,
                    message = "Fotos de Toma de Suplementos guardadas correctamente",
                    archivos = savedFiles
                });
            }
            catch (Exception ex)
            {                
                var exactError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new
                {
                    ok = false,
                    message = $"Error al guardar archivos: {exactError}"
                });
            }
        }
    }
}