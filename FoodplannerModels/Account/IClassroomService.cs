using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IClassroomService
    {
        Task<IEnumerable<Classroom>> GetAllClassroomAsync();
        Task<int> InsertClassroomAsync(CreateClassroomDTO classroom);

        Task<int> UpdateClassroomAsync(CreateClassroomDTO classroom, int id);

        Task<bool> CheckChildrenInClassroom(int id);

        Task<int> DeleteClassroomAsync(int id);

    }
}
