namespace FoodplannerServices.Hubs;

public interface IFeedbackChatClient
{
    Task ReceiveMessage(string user, string message);
}


