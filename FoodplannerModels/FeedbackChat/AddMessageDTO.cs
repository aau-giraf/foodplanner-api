namespace FoodplannerModels.FeedbackChat;

public class AddMessageDTO
{
    public int UserId { get; set; }
    public int ChildId { get; set; }
    public string Content { get; set; }
}