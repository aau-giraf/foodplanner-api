using Dapper;
using FoodplannerModels.Lunchbox;

// Adds the IngredientRepository to the FoodplannerDataAccessSql.Lunchbox namespace
namespace FoodplannerDataAccessSql.Lunchbox;

//The repository for the Ingredient class.
public class IngredientsRepository(PostgreSQLConnectionFactory connectionFactory)
{
    // Asynchronously retrieves all ingredients from the database. Used in testing.
    public async Task<IEnumerable<IngredientDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM ingredients";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<IngredientDTO>(sql);
        }
    }

    // Asynchronously retrieves all ingredients by user.
    public async Task<IEnumerable<IngredientDTO>> GetAllByUserAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE user_id = @Id";
        using (var connection = connectionFactory.Create())
        {
            return await connection.QueryAsync<IngredientDTO>(sql, new { Id = id });
        }
    }

    // Asynchronously retrieves an ingredient by its unique ID.
    public async Task<IngredientDTO?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<IngredientDTO>(sql, new { Id = id });
            connection.Close();
            return result;
        }
    }

    // Asynchronously inserts a new ingredient into the database and returns its Id.
    public async Task<int> InsertAsync(IngredientDTO entity)
    {
        var sql = "INSERT INTO ingredients (name, user_id, food_image_id) VALUES (@Name, @UserId, @FoodImageId) RETURNING id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            
            // If Food_image_id is null, we need to pass DBNull.Value to the database.
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
            });
        }
    }

    // Asynchronously updates an existing ingredient in the database.
    public async Task<int> UpdateAsync(IngredientDTO entity)
    {
        var sql = "UPDATE ingredients SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id
            });
        }
    }

    // Asynchronously deletes an ingredient from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM ingredients WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}