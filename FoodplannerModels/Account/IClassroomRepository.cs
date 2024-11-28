namespace FoodplannerModels.Account
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetAllAsync();
        Task<int> InsertAsync(CreateClassroomDTO createClassroomDTO);

        Task<int> UpdateAsync(CreateClassroomDTO createClassroomDTO, int id);

        Task<bool> CheckChildrenInClassroom(int id);

        Task<int> DeleteAsync(int id);
    }

}
