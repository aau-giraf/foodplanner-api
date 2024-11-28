using FoodplannerModels.FeedbackChat;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FoodplannerModels.Account;


namespace FoodplannerServices.FeedbackChat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IChildrenRepository _childrenRepository;

        public ChatService(IChatRepository chatRepository, IMapper mapper, IChildrenRepository childrenRepository)
        {
            _childrenRepository = childrenRepository;
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        // Methods for ChatThread
        public async Task<bool> AddMessageAsync(AddMessageDTO messageDTO,int userId)
        {
            var message = _mapper.Map<Message>(messageDTO);
            message.Date = System.DateTime.Now;
            message.UserId = userId;
            
            await _chatRepository.AddMessageAsync(message);
            return true;
        }

        // Methods for Message
        public async Task<IEnumerable<UserNameFeedbackChatDTO>> GetMessagesAsync(int chatThreadId)
        {
            var result = await _chatRepository.GetMessagesByChatThreadIdAsync(chatThreadId);
            
            foreach (UserNameFeedbackChatDTO message in result)
            {
                if (message.Archived)
                {
                    message.Content = "Denne besked er blevet slettet.";
                }
            }
            return result;
        }
        
        public async Task<bool> UpdateMessageAsync(UpdateMessageDTO message)
        {
            var _message = _mapper.Map<Message>(message);
            
            await _chatRepository.UpdateMessageAsync(_message);
            return true;
        }

        public async Task<bool> ArchiveMessageAsync(int messageId)
        {
            await _chatRepository.ArchiveMessageAsync(messageId);
            return true;
        }
        
        public async Task<int> GetChatThreadIdByChildIdAsync(int ChildId)
        {
            var chatThreadId = await _chatRepository.GetChatThreadIdByChildIdAsync(ChildId);
            if (chatThreadId == 0)
            {
                chatThreadId = await _chatRepository.AddChatThreadIdByChildIdAsync(ChildId);
            }
            return chatThreadId;
        }
        
        public async Task<int> GetChatThreadIdByUserIdAsync(int UserId)
        {
            var childId = await _childrenRepository.GetChildIdByParentIdAsync(UserId);
            return await GetChatThreadIdByChildIdAsync(childId);
        }
    }
}