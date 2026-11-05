using Mapster;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public class UserProfile : IRegister
    {
        // Entity for user profile 
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserCreateDTO>();
            config.NewConfig<UserCreateDTO, User>();

            config.NewConfig<User, UserArchivedDTO>();
            config.NewConfig<UserArchivedDTO, User>();

            config.NewConfig<User, UserCredsDTO>();
            config.NewConfig<UserCredsDTO, User>();

            config.NewConfig<User, UserRoleDTO>();
            config.NewConfig<UserRoleDTO, User>();

            config.NewConfig<User, UserDTO>();
            config.NewConfig<UserDTO, User>();

            config.NewConfig<User, UserUpdateDTO>();
            config.NewConfig<UserUpdateDTO, User>();

            config.NewConfig<User, UserUpdateLoggedInDTO>();
            config.NewConfig<UserUpdateLoggedInDTO, User>();

            config.NewConfig<Login, LoginDTO>();
            config.NewConfig<LoginDTO, Login>();

            config.NewConfig<User, UserCreateChildDTO>();
            config.NewConfig<UserCreateChildDTO, User>();

            config.NewConfig<Children, ChildrenDTO>();
            config.NewConfig<ChildrenDTO, Children>();

            config.NewConfig<CreateClassroomDTO, Classroom>();
            config.NewConfig<Classroom, ClassroomDTO>();
            config.NewConfig<ClassroomDTO, Classroom>();
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
