using Dapper;
using FoodplannerModels.Image;
using Npgsql;

namespace FoodplannerDataAccessSql.Image;

public class FoodImageRepository(PostgreSQLConnectionFactory connectionFactory) : IFoodImageRepository
{
    public async Task<IEnumerable<FoodImage>> GetAllImagesAsync()
    {
        const string sql = "SELECT * FROM food_images";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<FoodImage>(sql);
            return result.ToList();
        }
    }

    public async Task<FoodImage> GetImageByIdAsync(int userId, string id)
    {
        const string sql = "SELECT * FROM food_images WHERE id = @id";
        try
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                var result = connection.QuerySingleOrDefault<FoodImage>(sql, new { id });
                if (result is null)
                {
                    throw new NullReferenceException("FoodImage not found");
                }

                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int> InsertImageAsync(string imageId, int userId, string imageName, string imageFileType, long fileSize)
    {
        int result;
        var sql = "INSERT INTO food_images (image_id, user_id, image_name, image_file_type, size)" + 
                  $"VALUES ({imageId}, {userId}, {imageName}, {imageFileType}, {fileSize})";

        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            result = await connection.ExecuteScalarAsync<int>(sql);
        }
        return result;
    }

    public async Task<int> DeleteImageAsync(int id)
    {
        var sql = $"DELETE FROM food_images WHERE id = {id}";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            connection.Execute(sql);
        }
        return id;
    }
}