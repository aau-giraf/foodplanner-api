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
            
            CreateMap<User, UserArchivedDTO>();
            CreateMap<UserArchivedDTO, User>();
            
            CreateMap<User, UserCredsDTO>();
            CreateMap<UserCredsDTO, User>();

            CreateMap<User, UserRoleDTO>();
            CreateMap<UserRoleDTO, User>();

            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();

            CreateMap<User, UserUpdateDTO>();
            CreateMap<UserUpdateDTO, User>();

            CreateMap<User, UserUpdateLoggedInDTO>();
            CreateMap<UserUpdateLoggedInDTO, User>();
            
            CreateMap<Login, LoginDTO>();
            CreateMap<LoginDTO, Login>();
        }
    }
}
