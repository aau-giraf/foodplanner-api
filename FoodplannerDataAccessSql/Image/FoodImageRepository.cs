using FoodplannerModels.Image;
using Dapper;

namespace FoodplannerDataAccessSql.Image;

public class FoodImageRepository : IFoodImageRepository
{
    // Set up a connection factory to connect to the PostgreSQL database
    private readonly CorePostgreSQLConnectionFactory _connectionFactory;
    public FoodImageRepository(CorePostgreSQLConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // Asynchronously retrieves all food_images with given image ID  
    public async Task<FoodImage?> GetByIdAsync(int foodImageId)
    {
        const string sql = "SELECT * FROM food_image WHERE id = @Id";
        await using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<FoodImage>(sql, new { Id = foodImageId });
            return result;
        }
    }

    // Asynchronously add a new food_image to database, returns its ID
    public async Task<int> InsertAsync(FoodImage entity)
    {
        const string sql = @"
            INSERT INTO food_image (image_data, user_id, image_name, image_file_type, size)
            VALUES (@ImageData, @UserId, @ImageName, @ImageFileType, @Size)
            RETURNING id";
        await using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, entity);
            return id; 
        }

    }

    // Asynchronously deletes a food image from databse given its ID 
    public async Task<int> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM food_image WHERE id = @Id";
        await using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected; 
        }
    }
}