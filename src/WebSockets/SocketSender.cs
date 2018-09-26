using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ViciniServer.WebSockets {
    public class SocketSender {
        private WebSocket socket;

        public SocketSender(WebSocket socket) {
            this.socket = socket;
        }

        public async Task SendTextAsync(string type, string message) {
            // TODO: package up in JSON

            if (socket.State != WebSocketState.Open) {
                return;
            }
            message = $"[{type}]: ${message}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(
                buffer: new ArraySegment<byte>(
                    array: encodedMessage,
                    offset: 0,
                    count: message.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        // TODO: Add binary message support
    }
}