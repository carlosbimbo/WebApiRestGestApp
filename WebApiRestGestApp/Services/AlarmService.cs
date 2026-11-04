using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApiRestGestApp.Data;
using WebApiRestGestApp.Models;

namespace WebApiRestGestApp.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        // Lista estática con los canales exactos de Node.js
        private readonly List<AlarmConfig> _alarmTimes = new List<AlarmConfig>
        {
            new AlarmConfig { IdAlar = 1, Hour = 10, Minute = 30, Message = "⏰ Mami ya toca mi hierro", Sound = "alerta_uno", ChannelId = "v8_alerta_uno" },
            new AlarmConfig { IdAlar = 2, Hour = 11, Minute = 0, Message = "⏰ Mami toma mi hierro por favor", Sound = "alerta_dos", ChannelId = "v8_alerta_dos" },
            new AlarmConfig { IdAlar = 3, Hour = 17, Minute = 30, Message = "⏰ Gracias mamita, tenemos más hierro", Sound = "alerta_tres", ChannelId = "v8_alerta_tres" },
            new AlarmConfig { IdAlar = 4, Hour = 15, Minute = 30, Message = "⏰ Mami ya toca mi hierro", Sound = "alerta_uno", ChannelId = "v8_alerta_uno" },
            new AlarmConfig { IdAlar = 5, Hour = 16, Minute = 0, Message = "⏰ Mami toma mi hierro por favor", Sound = "alerta_dos", ChannelId = "v8_alerta_dos" }
        };

        public AlarmService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task ProcessAlarmsAsync(string triggerSource = "MANUAL")
        {
            // Zona horaria de Perú
            TimeZoneInfo limaTz = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            DateTime nowLima = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTz);
            DateTime fechaHoy = nowLima.Date;

            Console.WriteLine($"⏱ {triggerSource} ejecutado: {nowLima:O}");

            var users = await _context.Users
                .Where(u => !string.IsNullOrEmpty(u.ExpoPushToken))
                .ToListAsync();

            Console.WriteLine($"👥 Usuarios con token: {users.Count}");

            foreach (var alarm in _alarmTimes)
            {
                var alarmTime = fechaHoy.AddHours(alarm.Hour).AddMinutes(alarm.Minute);
                if (nowLima < alarmTime) continue;

                foreach (var user in users)
                {
                    // REGLA DE ORO ANTI-SPAM: Validamos si ya se la entregamos a Expo hoy.
                    var alreadySent = await _context.AlarmExecutions
                        .AnyAsync(a => a.UserId == user.Id && a.IdAlar == alarm.IdAlar && a.Fecha == fechaHoy);

                    if (alreadySent) continue;

                    // Validaciones de negocio (Anemia, tomas previas)
                    bool isValid = await ValidateAlarmAsync(alarm, user.Id, fechaHoy);
                    if (!isValid) continue;

                    Console.WriteLine($"📤 Enviando alarma {alarm.IdAlar} a user {user.Id}");

                    // Enviar a Expo
                    var (expoResponse, retries) = await SendPushNotificationAsync(user.ExpoPushToken, alarm);

                    // Registrar para no volver a intentarlo hoy
                    var execution = new AlarmExecution
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        IdAlar = alarm.IdAlar,
                        Fecha = fechaHoy,
                        RespuExpo = expoResponse,
                        NroReinte = retries,
                        CreatedAt = nowLima
                    };

                    _context.AlarmExecutions.Add(execution);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"✅ Registro guardado → user {user.Id}, alarma {alarm.IdAlar}");
                }
            }
        }

        private async Task<bool> ValidateAlarmAsync(AlarmConfig alarm, Guid userId, DateTime fecha)
        {
            string fechaString = fecha.ToString("yyyy-MM-dd");

            var totalSuplementos = await _context.RegistroSuplementos
                .CountAsync(s => s.IdUser == userId && s.Fecha == fechaString);

            var etapa = await _context.EtapasGestacionales
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (etapa == null || string.IsNullOrEmpty(etapa.Hemoglo)) return false;
            if (!double.TryParse(etapa.Hemoglo, out double hemoglo)) return false;

            bool tieneAnemia = hemoglo < 11;
            bool valid = false;

            if (!tieneAnemia)
            {
                if (alarm.IdAlar == 4 || alarm.IdAlar == 5) valid = false;
                else if (alarm.IdAlar == 1 || alarm.IdAlar == 2) valid = totalSuplementos < 1;
                else if (alarm.IdAlar == 3) valid = totalSuplementos == 1;
            }
            else
            {
                if (alarm.IdAlar == 1 || alarm.IdAlar == 2 || alarm.IdAlar == 4 || alarm.IdAlar == 5) valid = totalSuplementos < 2;
                else if (alarm.IdAlar == 3) valid = totalSuplementos == 2;
            }

            return valid;
        }

        private async Task<(string Response, int Retries)> SendPushNotificationAsync(string? token, AlarmConfig alarm)
        {
            if (string.IsNullOrEmpty(token)) return ("Token vacío o nulo", 0);

            // =======================================================================
            // EL PAYLOAD PERFECTO PARA GARANTIZAR ENTREGA
            // priority: "high" -> Obliga al sistema operativo a despertar la pantalla.
            // ttl: 86400 -> Si no tiene internet, Google intentará por 24 horas exactas.
            // =======================================================================
            var payload = new
            {
                to = token,
                title = "💧👶 GestApp te recuerda",
                body = alarm.Message,
                sound = alarm.Sound,
                channelId = alarm.ChannelId,
                priority = "high",
                ttl = 86400,
                data = new { idalar = alarm.IdAlar }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            // Reintentos de RED (Por si tu servidor Linux pierde el internet momentáneamente)
            int maxRetries = 3;
            int attempts = 0;
            string lastResponse = "Error inicializando petición";

            for (int i = 0; i < maxRetries; i++)
            {
                attempts++;
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://exp.host/--/api/v2/push/send")
                    {
                        Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                    };

                    var response = await _httpClient.SendAsync(request);
                    lastResponse = await response.Content.ReadAsStringAsync();

                    // Si Expo nos responde que lo recibió bien, salimos del ciclo. No hacemos SPAM.
                    if (response.IsSuccessStatusCode && !lastResponse.Contains("\"status\":\"error\""))
                    {
                        break;
                    }
                    else
                    {
                        if (i < maxRetries - 1) await Task.Delay(2000);
                    }
                }
                catch (Exception ex)
                {
                    lastResponse = $"Excepción de red: {ex.Message}";
                    if (i < maxRetries - 1) await Task.Delay(2000);
                }
            }

            if (lastResponse.Length > 500)
            {
                lastResponse = lastResponse.Substring(0, 497) + "...";
            }

            return (lastResponse, attempts);
        }
    }
}