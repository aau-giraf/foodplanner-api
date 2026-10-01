namespace FoodplannerModels.Account
{
    // Interface for classroom repository
    public interface IClassroomRepository : IGenericRepository<Classroom>
    {
        Task<bool> CheckChildrenInClassroom(int id);

    }

}
