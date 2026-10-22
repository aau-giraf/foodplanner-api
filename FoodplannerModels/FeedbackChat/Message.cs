namespace FoodplannerModels.FeedbackChat;

// Represents a message in a feedback chat thread.
public class Message
{
    public required int MessageId { get; init; }
    public required string Content { get; set; }
    public required DateTime Date { get; init; }
    public required int UserId { get; init; }
    public required int ChatThreadId { get; init; }
    public required bool Archived { get; set; }
    public required bool IsEdited { get; set; }
}