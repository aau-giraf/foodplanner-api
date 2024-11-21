namespace FoodplannerModels.FeedbackChat
{
    public interface IChatService
    {
        // Methods for ChatThread
        Task<bool> AddMessageAsync(AddMessageDTO message);
        Task<int> GetChatThreadIdByChildIdAsync(int chatThreadId);
        Task<int> GetChatThreadIdByUserIdAsync(int UserId);
        
    

        // Methods for Message
        Task<IEnumerable<Message>> GetMessagesAsync(int chatThreadId);
        Task<bool> UpdateMessageAsync(UpdateMessageDTO message);
        Task<bool> ArchiveMessageAsync(int messageId);
        
    }
}