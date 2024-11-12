
using ESPService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ESPService.Services;

public class SignalRNotificationService(IHubContext<CodesHub> hubContext) : IWebsocketNotificationService
{
    public static readonly string CodeGeneratedEvent = "OnCodeGenerated";
    public static readonly string CodeUsedEvent = "OnCodeUsed";

    public async Task OnCodesGenerated(string[] codes)
    {
        await hubContext.Clients.All.SendAsync(CodeGeneratedEvent, codes);
    }

    public async Task OnCodeUsed(string code)
    {
        await hubContext.Clients.All.SendAsync(CodeUsedEvent, code);
    }
}
