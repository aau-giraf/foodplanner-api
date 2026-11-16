using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

// The repository for the Ingredient class.
public class IngredientsRepository(IDapperExecutor executor) : IIngredientsRepository
{
    // Asynchronously retrieves all ingredients by user.
    public async Task<IEnumerable<Ingredient>> GetAllByUserAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE user_id = @Id";

        return await executor.QueryAsync<Ingredient>(sql, new { Id = id });
    }

    // Asynchronously retrieves an ingredient by its unique ID.
    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM ingredients WHERE id = @Id";

        return await executor.QuerySingleOrDefaultAsync<Ingredient>(
            sql, new { Id = id });
    }

    // Asynchronously inserts a new ingredient into the database and returns its ID.
    public async Task<int> InsertAsync(Ingredient entity)
    {
        var sql = "INSERT INTO ingredients (name, user_id, image_id) VALUES (@Name, @UserId, @ImageId) RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new
        {
            Name = entity.Name,
            UserId = entity.User_id,
            ImageId = entity.Image_id ?? (object)DBNull.Value
        });
    }

    // Asynchronously updates an existing ingredient in the database.
    public async Task<int> UpdateAsync(Ingredient entity)
    {
        var sql = "UPDATE ingredients SET name = @Name, user_id = @UserId, image_id = @ImageId WHERE id = @Id";

        return await executor.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.User_id,
            ImageId = entity.Image_id
        });
    }

    // Asynchronously deletes an ingredient from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM ingredients WHERE id = @Id";

        return await executor.ExecuteAsync(sql, new { Id = id });
    }
}