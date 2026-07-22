using FoodplannerModels.FeedbackChat;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FoodplannerModels.Account;
using FoodplannerServices.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace FoodplannerServices.FeedbackChat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        private readonly IChildrenRepository _childrenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IChatRepository chatRepository, IMapper mapper, IChildrenRepository childrenRepository, IUserRepository userRepository, IHubContext<ChatHub> hubContext, ILogger<ChatService> logger)
        {
            _childrenRepository = childrenRepository;
            _chatRepository = chatRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        // Methods for ChatThread
        public async Task<bool> AddMessageAsync(AddMessageDTO messageDTO,int userId)
        {
            var message = _mapper.Map<Message>(messageDTO);
            message.Date = System.DateTime.Now;
            message.UserId = userId;

            await _chatRepository.InsertAsync(message);

            try
            {
                var messageDto = _mapper.Map<UserNameFeedbackChatDTO>(message);

                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    messageDto.FirstName = user.FirstName;
                }

                await _hubContext.Clients.Group($"thread-{message.ChatThreadId}").SendAsync("ReceiveMessage", messageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to broadcast new FeedbackChat message to group thread-{message.ChatThreadId}");
            }

            return true;
        }

        // Methods for Message
        public async Task<IEnumerable<UserNameFeedbackChatDTO>> GetMessagesAsync(int chatThreadId)
        {
            var messages = await _chatRepository.GetMessagesByChatThreadIdAsync(chatThreadId);
            var result = messages.Select(message => _mapper.Map<UserNameFeedbackChatDTO>(message));
            
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
            
            await _chatRepository.UpdateAsync(_message);
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
            // Get all children for this parent
            var children = await _childrenRepository.GetChildrenByParentIdAsync(UserId);
            var firstChild = children.FirstOrDefault();
            
            if (firstChild == null)
            {
                throw new InvalidOperationException("Bruger har ingen børn tilknyttet");
            }
            
            return await GetChatThreadIdByChildIdAsync(firstChild.ChildId);
        }
    }
}