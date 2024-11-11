using AutoMapper;
using FoodplannerApi.Helpers;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FoodplannerServices.Account;

public class ClassroomService : IClassroomService {
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository) {
       _classroomRepository = classroomRepository;
    }

    public async Task<IEnumerable<Classroom>> GetAllClassroomAsync()
    {
        var classroom = await _classroomRepository.GetAllAsync();
        return classroom;
    }
    public async Task<int> InsertClassroomAsync(CreateClassroomDTO classroom)
    {
        var id = await _classroomRepository.InsertAsync(classroom);
        return id;
    }

    public async Task<int> UpdateClassroomAsync(CreateClassroomDTO classroom, int id)
    {
        var _id = await _classroomRepository.UpdateAsync(classroom, id);
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



