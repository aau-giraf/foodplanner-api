using System.Net.Http.Json;
using FoodplannerModels.Account;

namespace Test.Contract;

public class CoreClient(HttpClient httpClient)
{
    // add a new URI here when writing a new Pact test for an endpoint
    private const string GetAllClassroomsPath = "/legacy/foodplanner/api/Classrooms/GetAll";

    // add a new method here when writing a new Pact test for an endpoint
    // replace the request type with the desired one. 
    public async Task<IEnumerable<ClassroomDTO>> GetAllClassroomsAsync()
    {
        var classrooms = await httpClient.GetFromJsonAsync<List<ClassroomDTO>>(GetAllClassroomsPath);
        return classrooms ?? [];
    }
}
