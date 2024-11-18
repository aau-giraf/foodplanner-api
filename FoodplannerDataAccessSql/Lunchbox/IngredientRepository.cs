using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Ingredient class.
*/
public class IngredientRepository (PostgreSQLConnectionFactory connectionFactory) : IIngredientRepository
{
    // Repository for managing ingredient data in the database.
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    // Asynchronously retrieves all ingredients from the database. Used in testing.
    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        var sql = "SELECT * FROM ingredients"; // SQL query to select all ingredients.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open();  // Open the connection to the database.
        return await connection.QueryAsync<Ingredient>(sql); // Execute the query and return all ingredients.
    }

    // Asynchronously retrieves all ingredients by user.
    public async Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE id = @Id"; // SQL query to select all ingredients.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open();  // Open the connection to the database.
        return await connection.QueryAsync<Ingredient>(sql, new{ Id = id }); // Execute the query and return all ingredients.
    }

    // Asynchronously retrieves an ingredient by its unique ID.
    public async Task<Ingredient> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE id = @Id"; // SQL query to select an ingredient by ID.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        var result = await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id }); // Execute the query and retrieve the ingredient.
        if (result == null) return null; // Return null if no ingredient is found.
        else return result; // Return the found ingredient.
    }

    // Asynchronously inserts a new ingredient into the database.
    public async Task<int> InsertAsync(Ingredient entity)
    {
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        var sql = "INSERT INTO ingredients (name, user_id, food_image_id) VALUES (@Name, @UserId, @FoodImageId) RETURNING id"; // SQL query for insertion.
        return await connection.QuerySingleAsync<int>(sql, new
        {
            Name = entity.Name, 
            UserId = entity.User_id, 
            FoodImageId = entity.Food_image_id 
        }); // Execute the insertion and return the number of affected rows.
    }

    // Asynchronously updates an existing ingredient in the database.
    public async Task<int> UpdateAsync(Ingredient entity, int id)
    {
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        var sql = "UPDATE ingredients SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId WHERE id = @Id"; // SQL query to update an ingredient.
        return await connection.ExecuteAsync(sql, new
        {
            Name = entity.Name, 
            UserId = entity.User_id, 
            FoodImageId = entity.Food_image_id, 
            Id = id
        }); // Execute the update and return the number of affected rows.
    }

    // Asynchronously deletes an ingredient from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM ingredients WHERE id = @Id"; // SQL query to delete an ingredient by ID.
        using var connection = _connectionFactory.Create(); // Create a new database connection.
        connection.Open(); // Open the connection to the database.
        return await connection.ExecuteAsync(sql, new { Id = id }); // Execute the deletion and return the number of affected rows.

    }
}
