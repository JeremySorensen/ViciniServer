using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ViciniServer.Hardware;
using ViciniServer.Sessions;

namespace ViciniServer.WebSockets {

    public class WebSocketMiddleware {
        private readonly RequestDelegate next;

        private ISessionManager sessionManager;

        public WebSocketMiddleware(RequestDelegate next, ISessionManager sessionManager) {
            this.next = next;
            this.sessionManager = sessionManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) {
                await next?.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
            var socketSender = new SocketSender(socket);

            var sid = sessionManager.CreateSession(socketSender);

            await Receive(socket, async(result, hid, buffer) => {
                if (result.MessageType == WebSocketMessageType.Text) {
                    await sessionManager.SocketMessageAsync(sid, hid, buffer);
                } else if (result.MessageType == WebSocketMessageType.Close) {
                    await sessionManager.CloseSessionAsync(sid);
                }
            });

            await next?.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, HardwareId, string> handleMessage)
        {
            while (socket.State == WebSocketState.Open) {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                string message = null;
                WebSocketReceiveResult result = null;
                try {
                    using (var ms = new MemoryStream()) {
                        do {
                            result = await socket.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        } while (!result.EndOfMessage);

                        ms.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(ms, Encoding.UTF8)) {
                            message = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }

                    var fields = message.Split(',');
                    var hardwareId = new HardwareId(fields[0]);

                    handleMessage(result, hardwareId, fields[1]);
                } catch (WebSocketException e) {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely) {
                        socket.Abort();
                    }
                }
            }
        }

    }
}