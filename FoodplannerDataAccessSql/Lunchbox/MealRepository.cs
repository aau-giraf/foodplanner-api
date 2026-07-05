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
    public async Task<Meal?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM meals WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<Meal>(sql, new { Id = id });
            connection.Close();
            return result;
        }
    }

    // Asynchronously inserts a new meal into the database and returns its Id.
    public async Task<int> InsertAsync(Meal entity)
    {
        var sql = "INSERT INTO meals (name, user_id, food_image_id, date, template) VALUES (@Name, @UserId, @FoodImageId, @Date, @Template) RETURNING id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
                Date = entity.Date,
                Template = entity.Template
            });
        }
    }

    // Asynchronously updates an existing meal in the database.
    public async Task<int> UpdateAsync(Meal entity)
    {
        var sql = "UPDATE meals SET name = @Name, user_id = @UserId, food_image_id = @FoodImageId, date = @Date, template = @Template WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = entity.Id,
                Name = entity.Name,
                UserId = entity.User_id,
                FoodImageId = entity.Food_image_id ?? (object)DBNull.Value,
                Date = entity.Date,
                Template = entity.Template
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

    // Gets all meals where template is set to 1
    public async Task<IEnumerable<Meal>> GetAllTemplatesByUserAsync(int userId)
    {
        var sql = "SELECT * FROM meals WHERE template = true AND user_id = @UserId";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<Meal>(sql, new { UserId = userId });
        }
    }


    // Update a meals template status via its ID
    public async Task<int> UpdateTemplateStatusAsync(int id, bool template)
    {
        var sql = "UPDATE meals SET template = @template WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id, Template = template });
        }
    }

}
