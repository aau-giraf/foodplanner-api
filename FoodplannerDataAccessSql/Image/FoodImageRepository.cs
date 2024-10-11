using Dapper;
using FoodplannerModels.Image;
using Npgsql;

namespace FoodplannerDataAccessSql.Image;

public class FoodImageRepository(PostgreSQLConnectionFactory connectionFactory) : IFoodImageRepository
{
    public async Task<IEnumerable<FoodImage>> GetAllImagesAsync()
    {
        const string sql = "SELECT * FROM food_image";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<FoodImage>(sql);
            return result.ToList();
        }
    }

    public async Task<FoodImage> GetImageByIdAsync(int foodImageId)
    {
        string sql = $"SELECT * FROM food_image WHERE id = {foodImageId}";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = connection.QuerySingleOrDefault<FoodImage>(sql);
            if (result is null)
            {
                throw new NullReferenceException("FoodImage not found");
            }

            return result;
        }
    }

    public async Task<int> InsertImageAsync(string imageId, int userId, string imageName, string imageFileType, long fileSize)
    {
        int result;
        var sql = "INSERT INTO food_image (image_id, user_id, image_name, image_file_type, size)" + 
                  $"VALUES ('{imageId}', {userId}, '{imageName}', '{imageFileType}', {fileSize})";

        await using (var connection = connectionFactory.Create())
        {
            connection.Open();
            result = await connection.ExecuteScalarAsync<int>(sql);
        }
        return result;
    }

    public async Task DeleteImageAsync(int id)
    {
        var sql = $"DELETE FROM food_image WHERE id = '{id}'";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            connection.Execute(sql);
        }
    }
}