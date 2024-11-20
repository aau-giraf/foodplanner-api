using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Ingredient class.
*/
public class IngredientRepository (PostgreSQLConnectionFactory connectionFactory) : IIngredientRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    // Asynchronously retrieves all ingredients from the database. Used in testing.
    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        var sql = "SELECT * FROM ingredients";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<Ingredient>(sql);
        }
    }

    // Asynchronously retrieves all ingredients by user.
    public async Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id)
    {
        var sql = $"SELECT * FROM ingredients WHERE user_ref = '{user_ref}'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<Ingredient>(sql);
        }
    }

    // Asynchronously retrieves an ingredient by its unique ID.
    public async Task<Ingredient> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM ingredients WHERE id = '{id}'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id });
            if (result == null) return null;
            else return result;
        }
    }

    // Asynchronously inserts a new ingredient into the database and returns its Id.
    public async Task<int> InsertAsync(Ingredient entity)
    {
        var sql = "INSERT INTO ingredients (name, user_ref, image_ref) VALUES (@Name, @User_ref, @Image_ref) RETURNING id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            //Since image_ref is nullable in the database, we cast it as a nullable object in a null-coalescing operator in order to return the correct form of null to the database.
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                User_ref = entity.User_ref,
                Image_ref = (object?)entity.Image_ref ?? DBNull.Value
            });
        }
    }

    // Asynchronously updates an existing ingredient in the database.
    public async Task<int> UpdateAsync(Ingredient entity, int id)
    {
        var sql = $"UPDATE ingredients SET name = @Name, user_ref = @User_ref, image_ref = @Image_ref WHERE id = '{id}'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Name = entity.Name,
                User_ref = entity.User_ref,
                Image_ref = entity.Image_ref
            });
        }
    }

    // Asynchronously deletes an ingredient from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM ingredients WHERE id = '{id}'";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
