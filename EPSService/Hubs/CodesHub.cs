using Microsoft.AspNetCore.SignalR;

namespace ESPService.Hubs;

public class CodesHub(ILogger<CodesHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation($"New client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        logger.LogInformation($"Client disconnected from hub: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}
