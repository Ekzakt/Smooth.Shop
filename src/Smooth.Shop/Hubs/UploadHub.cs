using Microsoft.AspNetCore.SignalR;

namespace Smooth.Shop.Hubs;

public class UploadHub : Hub
{
    public string GetConnectionId() => Context.ConnectionId;

    public async Task SendProgress(int progress)
    {
        await Clients.All.SendAsync("ReceiveProgress", progress);
    }
}
