using System.Data.Common;
using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodplannerServices.Account;

public class ClassroomService : IClassroomService 

{
    private readonly IMapper _mapper;
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository, IMapper mapper) {
        _mapper = mapper;
        _classroomRepository = classroomRepository;
    }

    public async Task<IEnumerable<Classroom>> GetAllClassroomAsync()
    {
        var classroom = await _classroomRepository.GetAllByClassAsync();
        return classroom;
    }
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO classroom)
    {
        var id = await _classroomRepository.InsertAsync(_mapper.Map<Classroom>(classroom));
        return id;
    }

    public async Task<int> UpdateClassroomAsync(CreateClassroomDTO classroom, int id)
    {
        var _id = await _classroomRepository.UpdateAsync(new Classroom{ClassId = id, ClassName = classroom.ClassName});
        return _id;
    }

    public async Task<bool> CheckChildrenInClassroom(int id)
    {
        var result = await _classroomRepository.CheckChildrenInClassroom(id);
        return result;
    }

    public async Task<int> DeleteClassroomAsync(int id)
    {
        var result = await _classroomRepository.DeleteAsync(id);
        return result;
    }
}



