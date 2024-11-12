namespace ESPService.Services;

public interface IWebsocketNotificationService
{
    Task OnCodesGenerated(string[] codes);

    Task OnCodeUsed(string code);

}
