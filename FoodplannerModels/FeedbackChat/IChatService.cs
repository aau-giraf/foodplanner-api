namespace FoodplannerModels.FeedbackChat;

public interface IChatService
{
    Task<bool> AddMessageAsync(AddMessageDTO addMessageDTO, int userId);
    Task<IEnumerable<MessageDTO>> GetMessagesAsync(int chatThreadId);
    Task<bool> EditMessageAsync(EditMessageDTO message);
    Task<bool> ArchiveMessageAsync(int messageId);
    Task<int> GetThreadIdByChildIdAsync(int ChildId);
    Task<IEnumerable<int>> GetThreadIdsByParentIdAsync(int UserId);
}