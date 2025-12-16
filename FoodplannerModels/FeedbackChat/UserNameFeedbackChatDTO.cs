namespace FoodplannerModels.FeedbackChat;

public class UserNameFeedbackChatDTO
{
    public string Content { get; set; }
    public string FirstName { get; set; }
    public DateTime Date { get; set; }
    public bool Archived { get; set; }
    public bool IsEdited { get; set; }
}