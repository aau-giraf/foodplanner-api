using AutoMapper;

namespace FoodplannerModels.Account;

public class ClassroomProfile : Profile
{
    public ClassroomProfile()
    {
        CreateMap<Classroom, ClassroomProfile>();
        CreateMap<ClassroomProfile, Classroom>();
    }
}