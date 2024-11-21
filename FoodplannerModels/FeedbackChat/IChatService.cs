namespace FoodplannerModels.FeedbackChat
{
    public interface IChatService
    {
        // Methods for ChatThread
        Task<ChatThread> GetChatThreadByIdAsync(int id);
        Task<bool> AddMessageAsync(AddMessageDTO message);
    

        // Methods for Message
        Task<Message> GetMessageByIdAsync(int MessageId);
        Task<IEnumerable<Message>> GetMessagesAsync(int chatThreadId);
        Task<bool> UpdateMessageAsync(Message message);
        Task<bool> ArchiveMessageAsync(int messageId);
    }
}