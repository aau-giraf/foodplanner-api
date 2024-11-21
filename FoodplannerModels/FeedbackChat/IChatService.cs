namespace FoodplannerModels.FeedbackChat
{
    public interface IChatService
    {
        // Methods for ChatThread
        Task<ChatThread> GetChatThreadByIdAsync(int id);
        Task<IEnumerable<ChatThread>> GetAllChatThreadsAsync();
        Task<bool> AddMessageToThread(AddMessageDTO message);
    

        // Methods for Message
        Task<Message> GetMessageByIdAsync(int MessageId);
        Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId);
        Task<bool> AddMessageAsync(Message message);
        Task<bool> UpdateMessageAsync(Message message);
        Task<bool> ArchiveMessageAsync(int messageId);
    }
}