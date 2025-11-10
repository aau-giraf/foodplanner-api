using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenRepository
    {
        Task<IEnumerable<Children>> GetAllAsync();
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId);
        Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId);
        Task<int> InsertAsync(Children entity);
        Task<int> UpdateAsync(Children entity);
        Task<int> DeleteAsync(int id);
        Task<Children> GetChildByIdAsync(int id);
        
        // Junction table methods
        Task<int> AddParentToChildAsync(int userId, int childId);
        Task<int> RemoveParentFromChildAsync(int userId, int childId);
        Task<int> AddTeacherToChildAsync(int userId, int childId);
        Task<int> RemoveTeacherFromChildAsync(int userId, int childId);
        Task<IEnumerable<User>> GetTeachersByChildIdAsync(int childId);
        Task<IEnumerable<Children>> GetChildrenByTeacherIdAsync(int teacherId);
    }
}
