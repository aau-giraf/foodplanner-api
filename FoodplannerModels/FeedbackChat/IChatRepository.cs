using FoodplannerModels;
using FoodplannerModels.FeedbackChat;

public interface IChatRepository : IGenericRepository<Message>
{
    // Methods for ChatThread
    Task<ChatThread> GetChatThreadByIdAsync(int chatThreadId);
    Task<int> GetChatThreadIdByChildIdAsync(int ChildId);
    Task<int> AddChatThreadIdByChildIdAsync(int ChildId);


    // Methods for Message
    Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId);
    Task ArchiveMessageAsync(int messageId);
}