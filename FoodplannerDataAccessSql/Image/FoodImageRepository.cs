using Dapper;
using FoodplannerModels;
using FoodplannerModels.Image;
using Npgsql;

namespace FoodplannerDataAccessSql.Image;

public class FoodImageRepository : IGenericRepository<FoodImage>
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    public FoodImageRepository(PostgreSQLConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<FoodImage>> GetAllAsync()
    {
        const string sql = "SELECT * FROM food_image";
        await using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<FoodImage>(sql);
        return result;
    }

    public async Task<FoodImage?> GetByIdAsync(int foodImageId)
    {
        const string sql = "SELECT * FROM food_image WHERE id = @Id";
        await using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QuerySingleOrDefaultAsync<FoodImage>(sql, new { Id = foodImageId });
        return result;
    }

    public async Task<int> InsertAsync(FoodImage entity)
    {
        const string sql = @"
            INSERT INTO food_image (image_id, user_id, image_name, image_file_type, size)
            VALUES (@ImageId, @UserId, @ImageName, @ImageFileType, @Size)
            RETURNING id";
        await using var connection = _connectionFactory.Create();
        connection.Open();
        var id = await connection.QuerySingleAsync<int>(sql, entity);
        return id;
    }

    public async Task<int> UpdateAsync(FoodImage entity)
    {
        const string sql = @"
            UPDATE food_image
            SET image_id = @ImageId,
                user_id = @UserId,
                image_name = @ImageName,
                image_file_type = @ImageFileType,
                size = @Size
            WHERE id = @Id";
        await using var connection = _connectionFactory.Create();
        connection.Open();
        var affected = await connection.ExecuteAsync(sql, entity);
        return affected;
    }

    public async Task<int> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM food_image WHERE id = @Id";
        await using var connection = _connectionFactory.Create();
        connection.Open();
        var affected = await connection.ExecuteAsync(sql, new { Id = id });
        return affected;
    }
}