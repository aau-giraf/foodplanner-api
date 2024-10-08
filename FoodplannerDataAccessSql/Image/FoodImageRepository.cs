using Dapper;
using FoodplannerModels.Image;
using Npgsql;

namespace FoodplannerDataAccessSql.Image;

public class FoodImageRepository(PostgreSQLConnectionFactory connectionFactory) : IFoodImageRepository
{
    public async Task<IEnumerable<FoodImage>> GetAllImagesAsync()
    {
        var sql = "SELECT * FROM food_images";
        using (var connection = connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<FoodplannerModels.Image.FoodImage>(sql);
            return result.ToList();
        }
    }

    public async Task<FoodImage> GetImageByIdAsync(string id)
    {
        var sql = "SELECT * FROM food_images WHERE id = @id";
        try
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                var result = connection.QuerySingleOrDefault<FoodImage>(sql, new { id });
                if (result is null)
                {
                    throw new NullReferenceException("Foodimage not found");
                    
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

    public async Task<string> InsertImageAsync(FoodImage foodImage)
    {
        
        var sql = "INSERT INTO food_images (ImageID, UserId, ImageName, ImageFileType, ImageSize)" + "VALUES (@ImageID, @UserId, @ImageName, @ImageFileType, @ImageSize, @ImageId)";
        
        try
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();
                
                await using var cmd = new NpgsqlCommand(sql, connection);
                 
                cmd.Parameters.AddWithValue("@ImageId", foodImage.ImageId);
                cmd.Parameters.AddWithValue("@ImageName", foodImage.ImageName);
                cmd.Parameters.AddWithValue("@ImageFileType", foodImage.ImageFileType);
                cmd.Parameters.AddWithValue("@ImageSize", foodImage.ImageSize);
                
                await cmd.ExecuteNonQueryAsync();
            }
            return foodImage.ImageId;
        }
        catch (NpgsqlException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    public async Task<int> DeleteImageAsync(int id)
    {
        var sql = "DELETE FROM food_images WHERE id = @id";

        try
        {
            using (var connection = connectionFactory.Create())
            {
                connection.Open();

                NpgsqlCommand sqlDeleteCommand = new NpgsqlCommand(sql, connection);

                sqlDeleteCommand.Parameters.AddWithValue("@id", id);
                sqlDeleteCommand.Prepare();

                sqlDeleteCommand.ExecuteNonQuery();
            }

            return id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }



}