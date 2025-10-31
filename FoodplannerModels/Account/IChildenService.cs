using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenService
    {
        Task<IEnumerable<Children>> GetAllChildrenAsync();
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<IEnumerable<Children>> GetChildrenByParentIdAsync(int parentId);
        Task<IEnumerable<User>> GetParentsByChildIdAsync(int childId);
        Task<int> UpdateChildrenAsync(Children children);
        Task<Children> GetChildFromChildIdAsync(int id);
        Task<int> AddParentToChildAsync(int userId, int childId);
        Task<int> RemoveParentFromChildAsync(int userId, int childId);
    }
}
