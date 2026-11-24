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

    // IFoodImageRepository specific methods (extended from IGenericRepository class)
    // Asynchronously runs GetAllSync from this class 
    public async Task<IEnumerable<FoodImage>> GetAllImagesAsync()
    {
        using var connection = _connectionFactory.Create();
        connection.Open();

        const string sql = "SELECT * FROM food_image";
        return await connection.QueryAsync<FoodImage>(sql);
    }

    // Asynchronously use the GetById method from this class 
    // And throw an exception if ID is not found 
    public async Task<FoodImage> GetImageByIdAsync(int foodImageId)
    {
        var result = await GetByIdAsync(foodImageId);
        return result ?? throw new KeyNotFoundException($"FoodImage with id {foodImageId} not found");
    }

    // Given attributes, create a new food image entity 
    // Asynchronously add the new food image entry to database 
    public async Task<int> InsertImageAsync(byte[] imageData, int userid, string imageName, string imageType, long imageStreamLength)
    {
        var entity = new FoodImage
        {
            ImageData = imageData,
            UserId = userid,
            ImageName = imageName,
            ImageFileType = imageType,
            Size = imageStreamLength
        };
        return await InsertAsync(entity);
    }

    // Asynchronously remove food image entry from database using DeleteAsync method from this class
    public async Task DeleteImageAsync(int imageId)
    {
        await DeleteAsync(imageId);
    }

    // Asynchronously get all food images, where is_stock_image is true 
    public async Task<IEnumerable<FoodImage>> GetAllStockImagesAsync()
    {
        const string sql = "SELECT * FROM food_image WHERE is_stock_image = true";
        await using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<FoodImage>(sql);
            return result; 
        }
    }
}