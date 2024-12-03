namespace FoodplannerModels.FeedbackChat;

public class UserNameFeedbackChatDTO
{
    public int MessageID { get; set; }
    public string Content { get; set; }
    public string FirstName { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public int ChatThreadId { get; set; }
    public bool Archived { get; set; }
    public bool IsEdited { get; set; }
}