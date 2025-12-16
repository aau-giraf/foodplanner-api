using AutoMapper;

namespace FoodplannerModels.Account;

public class ClassroomProfile : Profile
{
    public ClassroomProfile()
    {
        CreateMap<Classroom, ClassroomProfile>();
        CreateMap<ClassroomProfile, Classroom>();

        CreateMap<Classroom, CreateClassroomDTO>();
        CreateMap<CreateClassroomDTO, Classroom>();
        
        CreateMap<Classroom, ClassroomDTO>();
        CreateMap<ClassroomDTO, Classroom>();
    }
}