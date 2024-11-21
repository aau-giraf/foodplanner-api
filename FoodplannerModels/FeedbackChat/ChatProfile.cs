using AutoMapper;
using FoodplannerModels.Account;

namespace FoodplannerModels.FeedbackChat;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<AddMessageDTO, Message>();
        // add more as we go 
    }
}