using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerDataAccessSql.Account;

// Adds Preference Repository to the FoodplannerDataAccessSql.Tags namespace
public class PreferenceRepository(IDapperExecutor executor) : IPreferenceRepository
{
    // Method to insert a new preference into preference table 
    // Preference contains: child's id, their rating, food name, date and the id of the food image
    // Returns an ID
    public async Task<int> InsertAsync (Preference entity)
    {
        var sql = "INSERT INTO preference (child_id, rating, food_name, date, food_image_id) VALUES (@Child_id, @Rating, @Food_name, @Date, @Food_image_id) RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new {child_id = entity.Child_id, rating = entity.Rating, food_name = entity.Food_name, date = entity.Date, food_image_id = entity.Food_image_id});
    }


    // Given child id, return list of their preferences
    public async Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id)
    {
        var sql = "SELECT * FROM preferences WHERE child_id = @Id"; 
        return await executor.QueryAsync<PreferenceDTO>(sql, new {Id = id}); 
    }
}