using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
