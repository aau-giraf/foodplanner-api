using AutoMapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodplannerServices.Account;

public class ClassroomService : IClassroomService {
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;

    public ClassroomService(IClassroomRepository classroomRepository, IMapper mapper){
       _classroomRepository = classroomRepository;
       _mapper = mapper;
    }

    public async Task<IEnumerable<ClassroomDTO>> GetAllClassroomAsync()
    {
        var classroom = await _classroomRepository.GetAllAsync();
        return classroom.Select(m => _mapper.Map<ClassroomDTO>(m));
    }
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO createClassroomDto)
    {
        var classroom = _mapper.Map<Classroom>(createClassroomDto);
        var id = await _classroomRepository.InsertAsync(classroom);
        return id;
    }

    public async Task<int> UpdateClassroomAsync(CreateClassroomDTO createClassroomDto, int id)
    {
        var classroom = _mapper.Map<Classroom>(createClassroomDto);
        var resultId = await _classroomRepository.UpdateAsync(classroom);
        return resultId;
    }

    public async Task<bool> CheckChildrenInClassroom(int id)
    {
        var result = await _classroomRepository.CheckChildrenInClassroom(id);
        return result;
    }

    public async Task<int> DeleteClassroomAsync(int id)
    {
        var result = await _classroomRepository.DeleteAsync(id);
        return result;
    }
}



