using FoodplannerModels.FeedbackChat;

public interface IChatRepository
{
    // Methods for ChatThread
    Task<int> GetChatThreadIdByChildIdAsync(int ChildId);
    Task<int> AddChatThreadIdByChildIdAsync(int ChildId);
    

    // Methods for Message
    Task<IEnumerable<UserNameFeedbackChatDTO>> GetMessagesByChatThreadIdAsync(int chatThreadId);
    Task AddMessageAsync(Message message);
    Task UpdateMessageAsync(Message message);
    Task ArchiveMessageAsync(int messageId);
}