using AutoMapper;

namespace FoodplannerModels.Account;

public class ClassroomProfile : Profile
{
    public ClassroomProfile()
    {
        CreateMap<Classroom, CreateClassroomDTO>();
        CreateMap<CreateClassroomDTO, Classroom>();
    }
}