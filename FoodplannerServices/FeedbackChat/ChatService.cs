using FoodplannerModels.FeedbackChat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodplannerServices.FeedbackChat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        // Methods for ChatThread
        public async Task<ChatThread> GetChatThreadByIdAsync(int id)
        {
            return await _chatRepository.GetChatThreadByIdAsync(id);
        }

        public async Task<IEnumerable<ChatThread>> GetAllChatThreadsAsync()
        {
            return await _chatRepository.GetAllChatThreadsAsync();
        }

        public async Task AddMessageToThread(int MessageId, int ChatThreadId)
        {
            await _chatRepository.AddMessageToThread(MessageId, ChatThreadId);
        }

        // Methods for Message
        public async Task<Message> GetMessageByIdAsync(int MessageId)
        {
            return await _chatRepository.GetMessageByIdAsync(MessageId);
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatThreadIdAsync(int chatThreadId)
        {
            return await _chatRepository.GetMessagesByChatThreadIdAsync(chatThreadId);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _chatRepository.AddMessageAsync(message);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            await _chatRepository.UpdateMessageAsync(message);
        }

        public async Task ArchiveMessageAsync(int messageId)
        {
            await _chatRepository.ArchiveMessageAsync(messageId);
        }
    }
}