using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;

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
    public async Task<int> InsertAsync(Ingredient entity, IEnumerable<int>? categoryIds = null)
    {
        var sql = "INSERT INTO ingredients (name, user_id, image_id, is_stock_img) VALUES (@Name, @UserId, @ImageId, @IsStockImg) RETURNING id";

        var ingredientId = await executor.QuerySingleAsync<int>(sql, new
        {
            Name = entity.Name,
            UserId = entity.User_id,
            ImageId = entity.Image_id ?? (object)DBNull.Value,
            IsStockImg = entity.Is_stock_img
        });

        // Creates a tag for each selected category
        foreach(var categoryId in categoryIds ?? Enumerable.Empty<int>())
        {
            // Tag created based on ingredient id and category id
            var tag = new TagDTO
            {
                Ingredient_id = ingredientId,
                Category_id = categoryId
            };

            var tagsql = "INSERT INTO tags(ingredient_id, category_id) VALUES (@Ingredient_id, @Category_id)";

            await executor.ExecuteAsync(tagsql ,tag);
        }

        return ingredientId;
    }

    // Asynchronously updates an existing ingredient in the database.
    public async Task<int> UpdateAsync(Ingredient entity)
    {
        var sql = "UPDATE ingredients SET name = @Name, user_id = @UserId, image_id = @ImageId, is_stock_img = @IsStockImg WHERE id = @Id";

        return await executor.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.User_id,
            ImageId = entity.Image_id,
            IsStockImg = entity.Is_stock_img
        });
    }

    // Asynchronously deletes an ingredient from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM ingredients WHERE id = @Id";

        return await executor.ExecuteAsync(sql, new { Id = id });
    }
}