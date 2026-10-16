using Dapper;
using FoodplannerModels.Lunchbox;

// Adds the MealRepository to the FoodplannerDataAccessSql.Lunchbox namespace
namespace FoodplannerDataAccessSql.Lunchbox;

// The repository for the Meal class.
public class MealsRepository(PostgreSQLConnectionFactory connectionFactory)
{
    // Asynchronously retrieves all meals from the database. Used in testing.
    public async Task<IEnumerable<MealDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM meals";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<MealDTO>(sql);
        }
    }

    // Asynchronously retrieves all meals from the database based on user id and date.
    public async Task<IEnumerable<MealDTO>> GetAllByUserAndDateAsync(int id, string date)
    {
        var sql = "SELECT * FROM meals WHERE user_id = @UserId AND date = @Date";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<MealDTO>(sql, new { UserId = id, Date = date });
            return result;
        }
    }

    // Asynchronously retrieves an meal by its unique ID.
    public async Task<MealDTO?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM meals WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<MealDTO>(sql, new { Id = id });
            connection.Close();
            return result;
        }
    }

    // Asynchronously inserts a new meal into the database and returns its Id.
    public async Task<int> InsertAsync(MealCreateDTO entity, int userId)
    {
        var sql = "INSERT INTO meals (name, user_id, image_id, date, template) VALUES (@Name, @UserId, @ImageId, @Date, @Template) RETURNING id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = entity.Name,
                UserId = userId,
                ImageId = entity.Image_id ?? (object)DBNull.Value,
                Date = entity.Date,
                Template = entity.Template
            });
        }
    }

    // Asynchronously updates an existing meal in the database.
    public async Task<int> UpdateAsync(MealDTO entity)
    {
        var sql = "UPDATE meals SET name = @Name, user_id = @UserId, image_id = @ImageId, date = @Date, template = @Template WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Name = entity.Name,
                UserId = entity.User_id,
                ImageId = entity.Image_id ?? (object)DBNull.Value,
                Date = entity.Date,
                Template = entity.Template
            });
        }
    }

    // Asynchronously deletes an meal from the database by its ID.
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM meals WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

    // Gets all meals where template is set to 1
    public async Task<IEnumerable<MealDTO>> GetAllTemplatesByUserAsync(int userId)
    {
        var sql = "SELECT * FROM meals WHERE template = true AND user_id = @UserId";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<MealDTO>(sql, new { UserId = userId });
        }
    }

    // Update a meals template status via its ID
    public async Task<int> UpdateTemplateStatusAsync(int id, bool template)
    {
        var sql = "UPDATE meals SET template = @template WHERE id = @Id";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id, Template = template });
        }
    }
}