using AutoMapper;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;

// Adds the ClassroomService to the FoodplannerService.Account namespace 
namespace FoodplannerServices.Account;


public class ClassroomService : IClassroomService {

    // Declares read only fields
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;

    // Constructor
    public ClassroomService(IClassroomRepository classroomRepository, IMapper mapper){
       _classroomRepository = classroomRepository;
       _mapper = mapper;
    }

    // Retrieves all classrooms from database
    public async Task<IEnumerable<ClassroomDTO>> GetAllClassroomAsync()
    {
        var classroom = await _classroomRepository.GetAllAsync();
        return classroom.Select(m => _mapper.Map<ClassroomDTO>(m));
    }

    // Creates a new classroom
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO createClassroomDto)
    {
        var classroom = _mapper.Map<Classroom>(createClassroomDto);
        var id = await _classroomRepository.InsertAsync(classroom);
        return id;
    }

    // Updates classroom
    public async Task<int> UpdateClassroomAsync(CreateClassroomDTO createClassroomDto, int id)
    {
        var classroom = _mapper.Map<Classroom>(createClassroomDto);
        var resultId = await _classroomRepository.UpdateAsync(classroom);
        return resultId;
    }

    // Checks if there is children in the classroom 
    // Necessary for when trying to delete a classroom 
    public async Task<bool> CheckChildrenInClassroom(int id)
    {
        var result = await _classroomRepository.CheckChildrenInClassroom(id);
        return result;
    }

    // Deletes classroom
    public async Task<int> DeleteClassroomAsync(int id)
    {
        var result = await _classroomRepository.DeleteAsync(id);
        return result;
    }
}



