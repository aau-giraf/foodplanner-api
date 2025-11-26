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
            CreateMap<User, UserCreateDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
            CreateMap<UserCreateDTO, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => ParseUserRole(src.Role)));

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
