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

            CreateMap<User, UserCreateChildDTO>();
            CreateMap<UserCreateChildDTO, User>();
        }

        private static UserRole ParseUserRole(string roleString)
        {
            if (string.IsNullOrWhiteSpace(roleString))
            {
                throw new ArgumentException("Role cannot be null or empty");
            }

            if (Enum.TryParse<UserRole>(roleString, true, out var role))
            {
                return role;
            }

            throw new ArgumentException($"Invalid role value: '{roleString}'. Valid values are: Admin, Child, Teacher, Parent");
        }
    }
}
