using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenRepository : IGenericRepository<Children>
    {
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenClassesAsync();
        Task<Children> GetByParentIdAsync(int id);
        Task<int> GetChildIdByParentIdAsync(int id);
        Task<Children> GetChildByIdAsync(int id);
    }
}
