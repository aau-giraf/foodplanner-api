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

    public async Task<Meal> GetByNameAsync(string name)
    {
        var sql = $"SELECT * FROM meals WHERE meal_name = '{name}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Meal>(sql);
        return result.FirstOrDefault();
    }

    public async Task<int> InsertAsync(Meal entity)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"INSERT INTO {typeof(Meal).Name}s ({string.Join(", ", GetContent(entity))})\n";
        sql += $"VALUES ({string.Join(", ", GetContent(entity, false))})";
        return await connection.ExecuteAsync(sql, entity);
    }

    public Task<int> UpdateAsync(Meal entity)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(string name)
    {
        var sql = $"DELETE FROM meals WHERE meal_name = '{name}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.ExecuteAsync(sql, new { Name = name });
    }

    private IEnumerable<string> GetContent(Meal entity, bool properties = true)
    {
        if(properties) return typeof(Meal).GetProperties().Select(p => p.Name);
        else return typeof(Meal).GetProperties().Select(p => "'" + p.GetValue(entity) + "'");
    }
}
