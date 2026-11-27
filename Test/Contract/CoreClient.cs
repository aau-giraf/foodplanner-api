using System.Net.Http.Json;
using FoodplannerModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace Test.Contract;
// this is a mock core client. Its purpose is to test the wrapper logic around a core-adapter endpoint. 
// The pact tests will always call the core-adapter endpoint via the mock server. This means that core is never actually called. 
// For now, a core client is not generated from the API spec, so this mock core client suffices. 
public class CoreClient(HttpClient httpClient)
{
    // add a new URI here when writing a new Pact test for an endpoint
    private const string GetAllClassroomsPath = "/legacy/foodplanner/api/Classrooms/GetAll";
    private const string UpdateUserByIdPath = "/legacy/foodplanner/api/admin/UpdateUser/1";

    // add a new method here when writing a new Pact test for an endpoint
    // replace the request type with the desired one. 
    public async Task<IEnumerable<Classroom>> GetAllClassroomsAsync()
    {
        var classrooms = await httpClient.GetFromJsonAsync<List<Classroom>>(GetAllClassroomsPath);
        return classrooms ?? [];
    }

    public async Task<HttpResponseMessage> UpdateUserById()
    {
        HttpContent content = JsonContent.Create(new UserUpdateDTO
        {
            FirstName = "string",
            LastName = "string",
            Email = "user@example.com",
            Password = "string"
        });
        return await httpClient.PutAsync(UpdateUserByIdPath, content);
    }
}
