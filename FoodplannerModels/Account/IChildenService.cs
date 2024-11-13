using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IChildrenService
    {
        Task<IEnumerable<ChildrenGetAllDTO>> GetAllChildrenAsync();
        Task<int> CreateChildrenAsync(ChildrenCreateParentDTO children);
    }
}
