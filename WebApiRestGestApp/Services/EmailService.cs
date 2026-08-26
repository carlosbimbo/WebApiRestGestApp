using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WebApiRestGestApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Resend:ApiKey"] ?? throw new Exception("Falta la API Key de Resend en appsettings.json");
        }

        public async Task<bool> SendRecoveryEmailAsync(string toEmail, string newPassword, string nombuser)
        {
            // Usamos interpolación de C# ($"") para insertar las variables directamente en el HTML
            var currentYear = DateTime.Now.Year;
            var htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""UTF-8"">
            </head>
            <body style=""font-family: Arial, sans-serif; background-color: #FFF0F2; margin: 0; padding: 20px;"">
                <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; background-color: #FFFFFF; border-radius: 20px; overflow: hidden; box-shadow: 0 4px 15px rgba(233, 30, 99, 0.1);"">
                    <tr>
                        <td align=""center"" style=""background-color: #85268D; padding: 30px 20px;"">
                            <img src=""https://macrocorpsystem.com/logogestanew.png"" alt=""GestApp Logo"" width=""100"" style=""display: block; border-radius: 50%; border: 4px solid #FFFFFF;"">
                            <h1 style=""color: #FFFFFF; margin: 15px 0 0 0; font-size: 28px; font-weight: bold; letter-spacing: 1px;"">GestApp</h1>
                            <p style=""color: #FFD6E7; margin: 5px 0 0 0; font-size: 14px;"">Tomando el hierro que necesitas refuerzas el amor a tu bebe.</p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 40px 30px; text-align: center;"">
                            <h2 style=""color: #78284A; font-size: 22px; margin-bottom: 20px;"">¡Hola {nombuser}, futura mamá!</h2>
                            <p style=""color: #501A31; font-size: 16px; line-height: 1.6; margin-bottom: 30px;"">
                                Hemos recibido una solicitud para recuperar tu contraseña. Aquí tienes tu nueva clave temporal de 4 dígitos:
                            </p>
                            <div style=""background-color: #FFF0F5; border: 2px dashed #85268D; border-radius: 15px; padding: 20px; margin: 0 auto; width: 60%;"">
                                <span style=""font-size: 36px; font-weight: bold; color: #85268D; letter-spacing: 8px;"">
                                    {newPassword}
                                </span>
                            </div>
                            <p style=""color: #8C7A87; font-size: 14px; margin-top: 30px; line-height: 1.5;"">
                                Usa esta contraseña para iniciar sesión en la aplicación. Si tú no solicitaste este cambio, por favor ignora este correo.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td align=""center"" style=""background-color: #FAFAFA; padding: 20px; border-top: 1px solid #EEEEEE;"">
                            <p style=""color: #A0AEC0; font-size: 12px; margin: 0;"">
                                © {currentYear} GestApp. Todos los derechos reservados.<br>
                            </p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            ";

            var payload = new
            {
                from = "GestApp <no-reply@macrocorpsystem.com>",
                to = new[] { toEmail },
                subject = "🔑 Recuperación de Contraseña - GestApp 👶🍼",
                html = htmlContent
            };

            // Preparamos la petición HTTP hacia Resend
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Si Resend devuelve un error, capturamos el mensaje para enviarlo al frontend
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception(errorResponse);
            }

            return true;
        }
    }
}