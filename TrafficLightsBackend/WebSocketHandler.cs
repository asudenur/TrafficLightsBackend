using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System;

public class WebSocketHandler
{
    private readonly IHubContext<TrafficHub> _hubContext;

    public WebSocketHandler(IHubContext<TrafficHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task HandleAsync(WebSocket webSocket)
    {
        Console.WriteLine("[WebSocket] Yeni bir bağlantı kabul edildi.");
        var buffer = new byte[1024 * 4];
        var segment = new ArraySegment<byte>(buffer);
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(segment, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("[WebSocket] Bağlantı kapandı.");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                break;
            }
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("[WebSocket] Gelen mesaj: " + message);
                try
                {
                    var obj = JsonSerializer.Deserialize<object>(message);
                    await _hubContext.Clients.All.SendAsync("ReceiveTrafficState", obj);
                }
                catch
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveTrafficState", message);
                }
            }
        }
        Console.WriteLine("[WebSocket] HandleAsync sonlandı.");
    }
} 