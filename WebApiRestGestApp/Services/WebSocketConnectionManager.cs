using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiRestGestApp.Services
{
    public interface IWebSocketConnectionManager
    {
        void AddSocket(WebSocket socket);
        Task RemoveSocket(WebSocket socket);
        Task BroadcastMessageAsync(string message);
    }

    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        // Thread-safe collection para guardar todos los clientes móviles conectados
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(WebSocket socket)
        {
            _sockets.TryAdd(Guid.NewGuid().ToString(), socket);
        }

        public async Task RemoveSocket(WebSocket socket)
        {
            var id = GetId(socket);
            if (id != null)
            {
                _sockets.TryRemove(id, out _);
                if (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var pair in _sockets)
            {
                var socket = pair.Value;
                if (socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch
                    {
                        // Si falla al enviar (ej. perdió señal), lo removemos
                        await RemoveSocket(socket);
                    }
                }
            }
        }

        private string GetId(WebSocket socket)
        {
            foreach (var pair in _sockets)
            {
                if (pair.Value == socket)
                    return pair.Key;
            }
            return null;
        }
    }
}