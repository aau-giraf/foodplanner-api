using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/// <summary>
/// Repository implementation for SubIngredient data access
/// </summary>
public class SubIngredientRepository : ISubIngredientRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    public SubIngredientRepository(PostgreSQLConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<SubIngredient>> GetAllAsync()
    {
        var sql = "SELECT * FROM subingredients";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<SubIngredient>(sql);
        }
    }

    public async Task<IEnumerable<SubIngredient>> GetAllByUserAsync(int userId)
    {
        var sql = "SELECT * FROM subingredients WHERE user_id = @UserId";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<SubIngredient>(sql, new { UserId = userId });
        }
    }

    public async Task<SubIngredient> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM subingredients WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<SubIngredient>(sql, new { Id = id });
            return result;
        }
    }

    public async Task<int> InsertAsync(SubIngredientDTO entity, int userId)
    {
        var sql = "INSERT INTO subingredients (name, user_id, food_image_id) VALUES (@Name, @UserId, @FoodImageId) RETURNING id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                UserId = userId,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
            });
        }
    }

    public async Task<int> UpdateAsync(SubIngredient entity, int id)
    {
        var sql = "UPDATE subingredients SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value
            });
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM subingredients WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}