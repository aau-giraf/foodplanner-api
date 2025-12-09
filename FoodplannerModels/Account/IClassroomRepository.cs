using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodplannerModels.Account
{
    public interface IClassroomRepository : IGenericRepository<Classroom>
    {
        Task<IEnumerable<Classroom>> GetAllByClassAsync();/*
        Task<int> InsertAsync(CreateClassroomDTO createClassroomDTO);
        Task<int> UpdateAsync(CreateClassroomDTO createClassroomDTO, int id);
        Task<int> DeleteAsync(int id);*/

        Task<bool> CheckChildrenInClassroom(int id);

    }

}
