namespace FoodplannerModels.FeedbackChat;

public class AddMessageDTO
{
    public int SentByUserId { get; set; }
    public int ChatThreadId { get; set; }
    public string Content { get; set; }
}