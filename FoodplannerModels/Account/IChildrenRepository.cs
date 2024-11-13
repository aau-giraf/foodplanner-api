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
        Task<int> InsertAsync(Children entity);
        Task<int> GetParentIdByChildIdAsync(int id);
        Task<int> UpdateAsync(Children entity);

        Task<int> DeleteAsync(int id);
    }
}
