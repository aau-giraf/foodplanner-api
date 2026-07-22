using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FoodplannerServices.Hubs;

[Authorize(Roles = "Parent, Teacher")]
public class ChatHub : Hub
{
    public async Task JoinThread(int chatThreadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }

    public async Task LeaveThread(int chatThreadId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread-{chatThreadId}");
    }
}
