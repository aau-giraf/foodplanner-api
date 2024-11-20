public interface IChatRepository
{
    // Methods for ChatThread
    Task<ChatThread> GetChatThreadByIdAsync(int id);
    Task<IEnumerable<ChatThread>> GetAllChatThreadsAsync();
    Task AddChatThreadAsync(ChatThread chatThread);
    Task UpdateChatThreadAsync(ChatThread chatThread);
    Task DeleteChatThreadAsync(int id);

    // Methods for Message
    Task<Message> GetMessageByIdAsync(int id);
    Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId);
    Task AddMessageAsync(Message message);
    Task UpdateMessageAsync(Message message);
    Task DeleteMessageAsync(int id);
}