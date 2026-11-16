using FoodplannerModels.FeedbackChat;
public interface IChatRepository
{
    Task<ChatThread> GetThreadByIdAsync(int chatThreadId);
    Task<int?> GetThreadIdByChildIdAsync(int ChildId);
    Task<int> CreateThreadByChildIdAsync(int ChildId);
    Task<IEnumerable<Message>> GetMessagesByThreadIdAsync(int chatThreadId);
    Task<int> AddMessageAsync(AddMessageDTO message, DateTime date, int userId);
    Task<int> EditMessageAsync(EditMessageDTO message);
    Task ArchiveMessageAsync(int messageId);
}