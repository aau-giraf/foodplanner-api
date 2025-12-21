using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenService
    {
        Task<IEnumerable<ChildrenDTO>> GetAllChildrenAsync();
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<IEnumerable<ChildrenDTO>> GetChildrenByParentIdAsync(int parentId);
        Task<IEnumerable<UserDTO>> GetParentsByChildIdAsync(int childId);
        Task<int> CreateChildrenAsync(ChildrenCreateParentDTO children);
        Task<int> UpdateChildrenAsync(ChildrenDTO children);        Task<int> DeleteChildrenAsync(int id);
        Task<ChildrenDTO> GetChildFromChildIdAsync(int id);
        Task<int> AddParentToChildAsync(int userId, int childId);
        Task<int> RemoveParentFromChildAsync(int userId, int childId);
        Task<int> AddTeacherToChildAsync(int userId, int childId);
        Task<int> RemoveTeacherFromChildAsync(int userId, int childId);
        Task<IEnumerable<UserDTO>> GetTeachersByChildIdAsync(int childId);
        Task<IEnumerable<ChildrenDTO>> GetChildrenByTeacherIdAsync(int teacherId);
    }
}
