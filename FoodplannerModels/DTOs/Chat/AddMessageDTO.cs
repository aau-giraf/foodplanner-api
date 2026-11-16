namespace FoodplannerModels.FeedbackChat;

// DTO for adding a message to a chat thread
public class AddMessageDTO
{
    public required int ChatThreadId { get; set; }
    public required string Content { get; set; }
}