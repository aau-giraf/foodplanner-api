namespace FoodplannerModels.Account
{
    public interface IClassroomService
    {
        Task<IEnumerable<Classroom>> GetAllClassroomAsync();
        Task<int> InsertClassroomAsync(CreateClassroomDTO classroom);

        Task<int> UpdateClassroomAsync(CreateClassroomDTO classroom, int id);

        Task<bool> CheckChildrenInClassroom(int id);

        Task<int> DeleteClassroomAsync(int id);

    }
}
