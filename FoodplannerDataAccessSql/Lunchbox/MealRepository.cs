using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Meal class.
*/
public class MealRepository(PostgreSQLConnectionFactory connectionFactory) : IMealRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    // Asynchronously retrieves all meals from the database. Used in testing.
    public async Task<IEnumerable<Meal>> GetAllAsync()
    {
        var sql = "SELECT * FROM meals";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<Meal>(sql);
        }
    }

    // Asynchronously retrieves all meals from the database that share the same user.
    public async Task<IEnumerable<Meal>> GetAllByUserAsync(int id, string date)
    {
        var sql = "SELECT * FROM meals WHERE user_id = @UserId AND date = @Date";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Meal>(sql, new { UserId = id, Date = date });
            return result;
        }
    }

    // Asynchronously retrieves an meal by its unique ID.
    public async Task<Meal> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM meals WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Meal>(sql, new { Id = id });
            if (result == null) return null;
            else return result;
        }
    }

    // Asynchronously inserts a new meal into the database and returns its Id.
    public async Task<int> InsertAsync(Meal entity, int id)
    {
        var sql = "INSERT INTO meals (name, user_id, food_image_id, date) VALUES (@Name, @UserId, @FoodImageId, @Date) RETURNING id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            //Since food_image_id is nullable in the database, we cast it as a nullable object in a null-coalescing operator in order to return the correct form of null to the database.
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                UserId = id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
                Date = entity.Date
            });
        }
    }

    // Asynchronously updates an existing meal in the database.
    public async Task<int> UpdateAsync(Meal entity, int id)
    {
        var sql = "UPDATE meals SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId, date = @Date WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
                Date = entity.Date
            });
        }
    }

    // Asynchronously deletes an meal from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM meals WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
