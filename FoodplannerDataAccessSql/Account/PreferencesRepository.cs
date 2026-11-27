using FoodplannerModels.Account;
using FoodplannerModels.Preferences;

namespace FoodplannerDataAccessSql.Account;

// Adds Preference Repository to the FoodplannerDataAccessSql.Tags namespace
public class PreferencesRepository(IDapperExecutor executor) : IPreferenceRepository
{
    // Method to insert a new preference into preference table 
    // Preference contains: child's id, their rating, food name, date and the id of the food image
    // Returns an ID
    public async Task<int> InsertAsync(Preference entity)
    {
        var sql = "INSERT INTO preferences (child_id, rating, food_name, date, food_image_id) VALUES (@Child_id, @Rating, @Food_name, @Date, @Food_image_id) RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new {child_id = entity.Child_id, rating = entity.Rating, food_name = entity.Food_name, date = entity.Date, food_image_id = entity.Food_image_id});
    }

    // Method for updating the preferences based on preferences ID or child ID
    // Returns ID of preference 
    public async Task<int> UpdateAsync(Preference entity)
    {
        var sql = "UPDATE preferences SET rating = @Rating, food_name = @Food_name, date = @Date, Food_image_id = @Food_image_id WHERE id = @Id, child_id = @Child_id RETURNING id";
        return await executor.QuerySingleAsync<int>(sql, new {rating = entity.Rating, food_name = entity.Food_name, date = entity.Date, food_image_id = entity.Food_image_id});
    }
    // MEthod for deleting preferences from preferences table based on ID
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETION FROM preferences WHERE id = @id";

        return await executor.ExecuteAsync(sql, new{Id = id});
    }


    // Given child id, return list of their preferences
    public async Task<IEnumerable<PreferenceDTO>> GetPreferencesByChildIdAsync(int id)
    {
        var sql = "SELECT * FROM preferences WHERE child_id = @Id"; 
        return await executor.QueryAsync<PreferenceDTO>(sql, new {Id = id}); 
    }
    
    
}