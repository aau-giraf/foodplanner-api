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
        Task<Children> GetByParentIdAsync(int id);
        Task<int> GetChildIdByParentIdAsync(int id);
        Task<int> InsertAsync(Children entity);
        Task<int> UpdateAsync(Children entity);
        Task<int> DeleteAsync(int id);
        Task<Children> GetChildByIdAsync(int id);
    }
}
