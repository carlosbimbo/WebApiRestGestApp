using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Controllers
{
    [ApiController]
    public class AlarmController : ControllerBase
    {
        private readonly IAlarmService _alarmService;

        public AlarmController(IAlarmService alarmService)
        {
            _alarmService = alarmService;
        }

        // =====================================================================
        // COMENTAR ESTA LÍNEA PARA APAGAR EL PROCESO AUTOMÁTICO EN SEGUNDO PLANO 27082026 de envio de notificaciones
        // =====================================================================
        
        [AllowAnonymous]
        [HttpPost("api/cenan2026/alarmsgesta")]
        public async Task<IActionResult> RunAlarmManually()
        {
            try
            {
                await _alarmService.ProcessAlarmsAsync("MANUAL");

                return Ok(new
                {
                    status = "OK",
                    message = "Alarmas procesadas correctamente"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error alarm controller: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "ERROR",
                    message = "Error procesando alarmas"
                });
            }
        }
        
    }
}