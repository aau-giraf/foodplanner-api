namespace FoodplannerModels.FeedbackChat;

public class AddMessageDTO
{
    public required int ChatThreadId { get; set; }
    public required string Content { get; set; }
}