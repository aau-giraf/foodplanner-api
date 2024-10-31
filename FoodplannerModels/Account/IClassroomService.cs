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

    }
}
