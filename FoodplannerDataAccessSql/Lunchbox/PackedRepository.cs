using Dapper;
using FoodplannerModels.Lunchbox;

// Adds the PackedIngredientRepository to the FoodplannerDataAccessSql.Lunchbox namespace
namespace FoodplannerDataAccessSql.Lunchbox;

// Repository for the PackedIngredient class.
public class PackedRepository(PostgreSQLConnectionFactory connectionFactory) : IPackedRepository
{
    // Get all packed ingredients from the database. Used in testing.
    public async Task<IEnumerable<PackedDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM packed";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<PackedDTO>(sql);
        }
    }

    // Get all packed ingredients associated with meal id
    public async Task<IEnumerable<PackedDTO>> GetAllByMealIdAsync(int id)
    {
        var sql = "SELECT * FROM packed WHERE meal_id = @Id ORDER BY order_number";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<PackedDTO>(sql, new { Id = id });
            connection.Close();
            return result;
        }
    }

    // Inserts a new packed ingredient into the database
    public async Task<int> InsertAsync(PackedDTO entity)
    {
        var sql = "INSERT INTO packed (meal_id, ingredient_id, order_number) VALUES (@MealId, @IngredientId, @OrderNumber)";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                MealId = entity.Meal_id,
                IngredientId = entity.Ingredient_id,
                OrderNumber = entity.Order_number
            });
        }
    }

    // Updates an existing packed ingredient
    public async Task<int> UpdateAsync(PackedDTO Old, PackedDTO New)
    {
        var sql = "UPDATE packed SET meal_id = @NewMealId, ingredient_id = @NewIngredientId, order_number = @NewOrderNumber WHERE meal_id = @OldMealId AND ingredient_id = @OldIngredientId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                NewMealId = New.Meal_id,
                NewIngredientId = New.Ingredient_id,
                NewOrderNumber = New.Order_number,
                OldMealId = Old.Meal_id,
                OldIngredientId = Old.Ingredient_id
            });
        }
    }

    // Deletes a packed ingredient from the database
    public async Task<int> DeleteAsync(PackedDTO entity)
    {
        var sql = "DELETE FROM packed WHERE meal_id = @MealId AND ingredient_id = @IngredientId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                MealId = entity.Meal_id,
                IngredientId = entity.Ingredient_id
            });
        }
    }
}