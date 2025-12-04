using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ETC.EPAY.Integration.Backend
{
    public class WebSocketRefundConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        public void AddSocket(string id, WebSocket socket)
        {
            _sockets.TryAdd(id, socket);
        }

        public WebSocket? GetSocket(string id)
        {
            _sockets.TryGetValue(id, out var socket);
            return socket;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }

        public async Task RemoveSocketAsync(string id)
        {
            if (_sockets.TryRemove(id, out var socket))
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closed by the server",
                    CancellationToken.None);
            }
        }
    }
}
