using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class TrafficHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"[SignalR] Yeni bir client bağlandı: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine($"[SignalR] Client bağlantısı koptu: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendTrafficState(object state)
    {
        Console.WriteLine("[SignalR] Trafik verisi gönderildi: " + state.ToString());
        await Clients.All.SendAsync("ReceiveTrafficState", state);
    }
}