using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IClassroomRepository
    {
        Task<IEnumerable<Classroom>> GetAllAsync();
        Task<int> InsertAsync(Classroom classroom);

        Task<int> UpdateAsync(Classroom classroom, int id);

        Task<bool> CheckChildrenInClassroom(int id);

        Task<int> DeleteAsync(int id);
    }

}
