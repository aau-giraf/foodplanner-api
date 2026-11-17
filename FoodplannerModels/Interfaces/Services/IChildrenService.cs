namespace FoodplannerModels.Account
{
    // Interface for children service 
    public interface IChildrenService
    {
        Task<IEnumerable<ChildrenDTO>> GetAllChildrenAsync();
        Task<IEnumerable<ChildrenDTO>> GetChildrenByParentIdAsync(int parentId);
        Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId);
        Task<int> UpdateChildrenAsync(ChildrenDTO children);
        Task<ChildrenDTO> GetChildFromChildIdAsync(int id);
        Task<int> AddParentToChildAsync(int userId, int childId);
        Task<int> RemoveParentFromChildAsync(int userId, int childId);
        Task<int> AddTeacherToChildAsync(int userId, int childId);
        Task<int> RemoveTeacherFromChildAsync(int userId, int childId);
        Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId);
        Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId);
    }
}
