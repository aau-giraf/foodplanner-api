using AutoMapper;
using FoodplannerModels.Account;

namespace FoodplannerModels.FeedbackChat;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<Message, AddMessageDTO>();
        CreateMap<AddMessageDTO, Message>();
        
        CreateMap<Message, UpdateMessageDTO>();
        CreateMap<UpdateMessageDTO, Message>();
    }
}