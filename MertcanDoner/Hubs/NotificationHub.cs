using Microsoft.AspNetCore.SignalR;

namespace MertcanDoner.Hubs
{
    public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userId = Context.UserIdentifier;

            if (user.IsInRole("Admin"))
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        await base.OnConnectedAsync();
    }
}
}
