using AutoMapper;

namespace FoodplannerModels.Account
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserCreateDTO>();
            CreateMap<UserCreateDTO, User>();
        }
    }
}
