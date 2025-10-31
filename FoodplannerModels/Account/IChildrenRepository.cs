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
        Task<int> InsertAsync(ChildrenCreateDTO entity);
        Task<int> UpdateAsync(Children entity);
        Task<Children> GetChildByIdAsync(int id);
        
        // Junction table methods
        Task<int> AddParentToChildAsync(int userId, int childId);
        Task<int> RemoveParentFromChildAsync(int userId, int childId);
    }
}
