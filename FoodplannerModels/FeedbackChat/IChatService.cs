namespace FoodplannerModels.FeedbackChat
{
    public interface IChatService
    {
        // Methods for ChatThread
        Task<ChatThread> GetChatThreadByIdAsync(int id);
        Task<bool> AddMessage(AddMessageDTO message);
    

        // Methods for Message
        Task<Message> GetMessageByIdAsync(int MessageId);
        Task<IEnumerable<Message>> GetMessagesAsync(int chatThreadId);
        Task<bool> AddMessageAsync(Message message);
        Task<bool> UpdateMessageAsync(Message message);
        Task<bool> ArchiveMessageAsync(int messageId);
    }
}