namespace FoodplannerModels.FeedbackChat;

public class UserNameFeedbackChatDTO
{
    public required string Content { get; set; }
    public required string FirstName { get; set; }
    public DateTime Date { get; set; }
    public bool Archived { get; set; }
    public bool IsEdited { get; set; }
}