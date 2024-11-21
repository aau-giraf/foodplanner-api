public interface IChatRepository
{
    // Methods for ChatThread
    Task<ChatThread> GetChatThreadByIdAsync(int id);
    Task<int> GetChatThreadIdByChildIdAsync(int ChildId);
    Task<int> AddChatThreadIdByChildIdAsync(int ChildId);
    

    // Methods for Message
    Task<Message> GetMessageByIdAsync(int MessageId);
    Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId);
    Task AddMessageAsync(Message message);
    Task UpdateMessageAsync(Message message);
    Task ArchiveMessageAsync(int messageId);
}