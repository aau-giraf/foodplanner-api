﻿using FoodplannerModels.FeedbackChat;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;


namespace FoodplannerServices.FeedbackChat
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;

        public ChatService(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        // Methods for ChatThread
        public async Task<ChatThread> GetChatThreadByIdAsync(int id)
        {
            return await _chatRepository.GetChatThreadByIdAsync(id);
        }
        
        public async Task<bool> AddMessageAsync(AddMessageDTO messageDTO)
        {
            var message = _mapper.Map<Message>(messageDTO);
            message.Date = System.DateTime.Now;
            var chatThreadId = await _chatRepository.GetChatThreadIdByChildIdAsync(messageDTO.ChildId);
            message.ChatThreadId = chatThreadId;
            await _chatRepository.AddMessageAsync(message);
            return true;
        }

        // Methods for Message
        public async Task<Message> GetMessageByIdAsync(int MessageId)
        {
            return await _chatRepository.GetMessageByIdAsync(MessageId);
        }
        
        public async Task<IEnumerable<Message>> GetMessagesAsync(int ChildId)
        {
            
            var chatThreadId = await _chatRepository.GetChatThreadIdByChildIdAsync(ChildId);
            if (chatThreadId == 0)
            {
                chatThreadId = await _chatRepository.AddChatThreadIdByChildIdAsync(ChildId);
            }
            var result = await _chatRepository.GetMessagesByChatThreadIdAsync(chatThreadId);
            
            foreach (Message message in result)
            {
                if (message.Archived)
                {
                    message.Content = "";
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
    }
}