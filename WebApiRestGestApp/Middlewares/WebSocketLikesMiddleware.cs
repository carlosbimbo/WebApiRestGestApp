using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using WebApiRestGestApp.Services;

namespace WebApiRestGestApp.Middlewares
{
    public class WebSocketLikesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketConnectionManager _manager;

        public WebSocketLikesMiddleware(RequestDelegate next, IWebSocketConnectionManager manager)
        {
            _next = next;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Intercepta las peticiones WebSocket. 
            // NOTA: Ajusta el context.Request.Path si tu ruta en Node.js no era la raíz.
            // Según tu app React Native es: WS_URL = 'wss://www.macrocorpsystem.com/cenan2025'
            if (context.Request.Path == "/cenan2025" && context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _manager.AddSocket(webSocket);

                // Mantiene el socket abierto escuchando (Keep-Alive)
                await ReceiveMessages(webSocket);
            }
            else
            {
                // Si no es un WebSocket, continúa el flujo normal HTTP
                await _next(context);
            }
        }

        private async Task ReceiveMessages(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _manager.RemoveSocket(socket);
                    }
                }
            }
            catch (Exception)
            {
                await _manager.RemoveSocket(socket);
            }
        }
    }
}