using AutoMapper;
using FoodplannerModels.Account;

namespace FoodplannerModels.FeedbackChat;

// AutoMapper profile for mapping between Message, AddMessageDTO, UserNameFeedbackChatDTO, UpdateMessageDTO, and ChatThreadDTO.
public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Message, AddMessageDTO>();
        CreateMap<AddMessageDTO, Message>();
        
        CreateMap<Message, UserNameFeedbackChatDTO>();
        CreateMap<UserNameFeedbackChatDTO, Message>();
        
        CreateMap<Message, UpdateMessageDTO>();
        CreateMap<UpdateMessageDTO, Message>();
        
        CreateMap<ChatThread, ChatThreadDTO>();
        CreateMap<ChatThreadDTO, ChatThread>();
    }
}