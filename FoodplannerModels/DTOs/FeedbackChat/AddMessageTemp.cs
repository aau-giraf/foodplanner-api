namespace FoodplannerModels.FeedbackChat;

public class AddMessageTemp
{
    public required int ChatThreadId { get; init; }
    public required string Content { get; init; }
    public required DateTime Date { get; init;}
    public required int UserId { get; init;}
}