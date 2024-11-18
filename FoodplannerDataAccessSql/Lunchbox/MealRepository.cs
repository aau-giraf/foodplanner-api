using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Meal class.
*/
public class MealRepository (PostgreSQLConnectionFactory connectionFactory) : IMealRepository
{
    // Repository for managing meal data in the database.
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    // Asynchronously retrieves all meals from the database. Used in testing.
    public async Task<IEnumerable<Meal>> GetAllAsync()
    {
        var sql = "SELECT * FROM meals"; // SQL query to select all meals.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        return await connection.QueryAsync<Meal>(sql); // Execute the query and return all meals.
    }

    // Asynchronously retrieves all meals from the database that share the same user.
    public async Task<IEnumerable<Meal>> GetAllByUserAsync(int id, string date)
    {
        var sql = "SELECT * FROM meals WHERE user_id = @Id AND date = @Date"; // SQL query to select all meals by user.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        return await connection.QueryAsync<Meal>(sql, new { Id = id, Date = date }); // Execute the query and return all meals.
    }

    // Asynchronously retrieves an meal by its unique ID.
    public async Task<Meal> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM meals WHERE id = @Id"; // SQL query to select an meal by ID.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        var result = await connection.QuerySingleOrDefaultAsync<Meal>(sql, new{ Id = id }); // Execute the query and retrieve the meal.
        if (result == null) return null; // Return null if no meal is found.
        else return result; // Return the found meal.
    }

    // Asynchronously inserts a new meal into the database.
    public async Task<int> InsertAsync(Meal entity)
    {
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        var sql = "INSERT INTO meals (name, user_id, food_image_id, date) VALUES (@Name, @UserId, @FoodImageId, @Date) RETURNING id"; // SQL query for insertion.
        return await connection.QuerySingleAsync<int>(sql, new
        {
            Name = entity.Name, 
            UserId = entity.User_id, 
            FoodImageId = entity.Food_image_id, 
            Date = entity.Date
        }); // Execute the insertion and return the number of affected rows.
    }

    // Asynchronously updates an existing meal in the database.
    public async Task<int> UpdateAsync(Meal entity, int id)
    {
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); //Open the connection to the database.
        var sql = "UPDATE meals SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId, date = @Date WHERE id = @Id"; // SQL query to update an meal.
        return await connection.ExecuteAsync(sql, new
        {
            Name = entity.Name, 
            UserId = entity.User_id, 
            FoodImageId = entity.Food_image_id, 
            Date = entity.Date, 
            Id = id
        }); // Execute the update and return the number of affected rows.
    }

    // Asynchronously deletes an meal from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM meals WHERE id = @Id"; // SQL query to delete an meal by ID.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        return await connection.ExecuteAsync(sql, new { Id = id }); // Execute the deletion and return the number of affected rows.
    }
}
