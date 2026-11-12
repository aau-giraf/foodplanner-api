using FoodplannerModels.FeedbackChat;
public interface IChatRepository
{
    // Methods for ChatThread
    Task<ChatThread> GetThreadByIdAsync(int chatThreadId);
    Task<int?> GetThreadIdByChildIdAsync(int ChildId);
    Task<int> CreateThreadByChildIdAsync(int ChildId);

    // Methods for Message
    Task<Message?> GetMessageByIdAsync(int messageId);
    Task<IEnumerable<Message>> GetMessagesByThreadIdAsync(int chatThreadId);
    Task<int> AddMessageAsync(AddMessageTemp message);
    Task<int> EditMessageAsync(EditMessageDTO message);
    Task ArchiveMessageAsync(int messageId);
    Task<IEnumerable<Message>> GetAllMessagesAsync();
    Task<int> DeleteMessageAsync(int id);
}