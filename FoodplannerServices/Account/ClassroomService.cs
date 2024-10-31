using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class ClassroomService : IClassroomService {
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository) {
       _classroomRepository = classroomRepository;
    }

    public async Task<IEnumerable<Classroom>> GetAllClassroomAsync()
    {
        var classroom = await _classroomRepository.GetAllAsync();
        return classroom;
    }
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO classroom)
    {
        var id = await _classroomRepository.InsertAsync(classroom);
        return id;
    }
}



