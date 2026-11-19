namespace FoodplannerModels.Account;

public interface IClassroomService
{
    Task<IEnumerable<Classroom>> GetAllClassroomAsync();
    Task<int> InsertClassroomAsync(string classroomName);
    Task<int> UpdateClassroomAsync(string classroomName, int id);
    Task<bool> CheckChildrenInClassroom(int id);
    Task<int> DeleteClassroomAsync(int id);
}