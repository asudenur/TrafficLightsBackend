var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

app.UseCors();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var wsHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await wsHandler.HandleAsync(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.MapHub<TrafficHub>("/trafficHub");

app.Run();