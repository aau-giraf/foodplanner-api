using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenRepository
    {
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllAsync();
        Task<int> InsertAsync(Children entity);
        Task<Children> GetByIdAsync(int id);
        Task<int> GetParentIdByChildIdAsync(int id);
    }
}
