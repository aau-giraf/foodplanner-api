namespace FoodplannerModels.FeedbackChat;

public class UpdateMessageDTO
{
    public int MessageId { get; set; }
    public required string Content { get; set; }
}