namespace FoodplannerModels.Account
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetAllAsync();
        Task<int> InsertAsync(Classroom entity);
        Task<int> UpdateAsync(Classroom entity);
        Task<bool> CheckChildrenInClassroom(int id);
        Task<int> DeleteAsync(int id);
    }
}
