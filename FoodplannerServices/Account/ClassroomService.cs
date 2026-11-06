using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;

// Adds the ClassroomService to the FoodplannerService.Account namespace 
namespace FoodplannerServices.Account;


public class ClassroomService : IClassroomService {

    // Declares read only fields
    private readonly IClassroomRepository _classroomRepository;

    // Constructor
    public ClassroomService(IClassroomRepository classroomRepository){
       _classroomRepository = classroomRepository;
    }

    // Retrieves all classrooms from database
    public async Task<IEnumerable<ClassroomDTO>> GetAllClassroomAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return classrooms.Select(classroom => new ClassroomDTO{ClassId = classroom.ClassId,ClassName = classroom.ClassName}).ToList();
    }

    // Creates a new classroom
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO createClassroomDto)
    {

        var classroom = new Classroom{ClassName = createClassroomDto.ClassName};

        return await _classroomRepository.InsertAsync(classroom);
    }

    // Updates classroom
    public async Task<int> UpdateClassroomAsync(CreateClassroomDTO createClassroomDto, int id)
    {
        var classroom = new Classroom{ClassId = id, ClassName = createClassroomDto.ClassName}; 

        return await _classroomRepository.UpdateAsync(classroom);
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



