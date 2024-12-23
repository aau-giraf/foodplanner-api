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
        Task<Children> GetChildrenByIdAsync(int id);
        Task<int> CreateChildrenAsync(ChildrenCreateParentDTO children);
        Task<int> UpdateChildrenAsync(Children children);

        Task<int> DeleteChildrenAsync(int id);
        Task<Children> GetChildFromChildIdAsync(int id);
    }
}
