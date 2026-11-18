using FoodplannerModels.Account;

namespace FoodplannerServices.Account;

public class ClassroomService(IClassroomRepository classroomRepository) : IClassroomService
{
    // Retrieves all classrooms from database
    public async Task<IEnumerable<Classroom>> GetAllClassroomAsync()
    {
        return await classroomRepository.GetAllAsync();
    }

    // Creates a new classroom
    public async Task<int> InsertClassroomAsync(string classroomName)
    {
        var classroom = new Classroom{ClassName = classroomName};
        return await classroomRepository.InsertAsync(classroom);
    }

    // Updates classroom
    public async Task<int> UpdateClassroomAsync(string classroomName, int id)
    {
        var classroom = new Classroom{ClassId = id, ClassName = classroomName}; 

        return await classroomRepository.UpdateAsync(classroom);
    }

    // Checks if there is children in the classroom 
    // Necessary for when trying to delete a classroom 
    public async Task<bool> CheckChildrenInClassroom(int id)
    {
        var result = await classroomRepository.CheckChildrenInClassroom(id);
        return result;
    }

    // Deletes classroom
    public async Task<int> DeleteClassroomAsync(int id)
    {
        var result = await classroomRepository.DeleteAsync(id);
        return result;
    }
}