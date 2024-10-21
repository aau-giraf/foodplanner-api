using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Meal class.
*/
public class MealRepository (PostgreSQLConnectionFactory connectionFactory) : IMealRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Meal>> GetAllAsync()
    {
        var sql = "SELECT * FROM meals";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.QueryAsync<Meal>(sql);
    }

    public async Task<Meal> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM meals WHERE id = '{id}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QuerySingleOrDefaultAsync<Meal>(sql, new { Id = id });
        if (result == null) return null;
        else return result;
    }

    public async Task<int> InsertAsync(Meal entity)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"INSERT INTO meals (title, user_ref, image_ref, date)\n";
        sql += $"VALUES ('{entity.Title}', '{entity.User_ref}', '{entity.Image_ref}', '{entity.Date}')";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(Meal entity, int id)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"UPDATE meals\n";
        sql += $"SET title = '{entity.Title}', user_ref = '{entity.User_ref}', image_ref = '{entity.Image_ref}', date = '{entity.Date}'";
        sql += $"WHERE id = '{id}'";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM meals WHERE id = '{id}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
