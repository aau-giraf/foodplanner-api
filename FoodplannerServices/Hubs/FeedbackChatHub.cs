using Microsoft.AspNetCore.SignalR;
namespace FoodplannerServices.Hubs;

public class FeedbackChatHub : Hub<IFeedbackChatClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.ReceiveMessage("System", "Feedback connected");
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.User(user).ReceiveMessage(user, message);
        // Add logic to save message to database
    }

    public async Task<bool> ArchiveMessageAsync(int messageId)
    {
        // Implement your logic to archive the message here
        return await Task.FromResult(true);
    }
}