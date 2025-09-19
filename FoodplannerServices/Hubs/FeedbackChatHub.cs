using Microsoft.AspNetCore.SignalR;
namespace FoodplannerServices.Hubs;

public class FeedbackChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", "Feedback connected");
    }


    public async Task NewMessage(long userId, string message)
    {
        await Clients.Client(Context.ConnectionId).SendAsync("RecivedMessage", userId, message);
    }

};