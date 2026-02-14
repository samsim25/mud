using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Collections.Concurrent;

namespace Avalon.Web
{
    public class MudWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private static ConcurrentDictionary<string, TcpClient> _clients = new();

        public MudWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
                {
                    var ws = await context.WebSockets.AcceptWebSocketAsync();
                    string host = context.Request.Query["host"];
                    int port = int.TryParse(context.Request.Query["port"], out var p) ? p : 4000;

                    using var tcp = new TcpClient();
                    try
                    {
                        await tcp.ConnectAsync(host, port);
                        var netStream = tcp.GetStream();
                        var buffer = new byte[4096];

                        // Start reading from MUD server
                        _ = Task.Run(async () =>
                        {
                            while (tcp.Connected)
                            {
                                int bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length);
                                if (bytesRead > 0)
                                {
                                    var msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), System.Net.WebSockets.WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                                }
                            }
                        });

                        // Read from websocket and send to MUD server
                        var wsBuffer = new byte[4096];
                        while (ws.State == System.Net.WebSockets.WebSocketState.Open)
                        {
                            var result = await ws.ReceiveAsync(new ArraySegment<byte>(wsBuffer), System.Threading.CancellationToken.None);
                            if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
                            {
                                await netStream.WriteAsync(wsBuffer, 0, result.Count);
                            }
                            else if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
                            {
                                await ws.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Closed", System.Threading.CancellationToken.None);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"[MudWebSocketMiddleware] Inner Exception: {ex}");
                        await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Error: {ex.Message}")), System.Net.WebSockets.WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                    }
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[MudWebSocketMiddleware] Outer Exception: {ex}");
                throw;
            }
        }
    }

    public static class MudWebSocketExtensions
    {
        public static IApplicationBuilder UseMudWebSocket(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MudWebSocketMiddleware>();
        }
    }
}
