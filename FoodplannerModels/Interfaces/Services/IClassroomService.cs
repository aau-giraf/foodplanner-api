namespace FoodplannerModels.Account
{
    // Interface for classroom service 
    public interface IClassroomService
    {
        Task<IEnumerable<ClassroomDTO>> GetAllClassroomAsync();
        Task<int> InsertClassroomAsync(CreateClassroomDTO classroom);

        Task<int> UpdateClassroomAsync(CreateClassroomDTO classroom, int id);

        Task<bool> CheckChildrenInClassroom(int id);

        Task<int> DeleteClassroomAsync(int id);

    }
}
