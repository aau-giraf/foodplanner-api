namespace FoodplannerModels.Account;

public interface IChildrenRepository
{
    Task<IEnumerable<Children>> GetAllAsync();
    Task<int> InsertAsync(Children entity);
    Task<int> UpdateAsync(Children entity);
    Task<Children?> GetByIdAsync(int id);
    Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId);
    Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId);
    Task<int> AddUserToChildAsync(int userId, int childId);
    Task<int> RemoveUserFromChildAsync(int userId, int childId);
    Task<IEnumerable<User>> GetTeachersByChildIdAsync(int childId);
    Task<IEnumerable<Children>> GetChildrenByTeacherIdAsync(int teacherId);
}