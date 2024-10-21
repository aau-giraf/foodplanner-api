using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/**
* The repository for the Ingredient class.
*/
public class IngredientRepository (PostgreSQLConnectionFactory connectionFactory) : IIngredientRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory = connectionFactory;

    public async Task<IEnumerable<Ingredient>> GetAllAsync()
    {
        var sql = "SELECT * FROM ingredients";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.QueryAsync<Ingredient>(sql);
    }

    public async Task<Ingredient> GetByNameAsync(string name, string user)
    {
        var sql = $"SELECT * FROM ingredients WHERE name = '{name}' AND user_ref = '{user}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Name = name });
        if (result == null) return null;
        else return result;
    }

    public async Task<int> InsertAsync(Ingredient entity)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"INSERT INTO {typeof(Ingredient).Name}s ({string.Join(", ", GetContent(entity))})\n";
        sql += $"VALUES ({string.Join(", ", GetContent(entity, false))})";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(Ingredient entity, string name, string user)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"UPDATE ingredients\n";
        sql += $"SET name = '{entity.Name}', user_ref = '{entity.User_ref}', image_ref = '{entity.Image_ref}'";
        sql += $"WHERE name = '{name}' AND user_ref = '{user}'";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(string name, string user)
    {
        var sql = $"DELETE FROM ingredients WHERE name = '{name}' AND user_ref = '{user}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.ExecuteAsync(sql, new { Name = name });
    }

    private IEnumerable<string> GetContent<T>(T entity, bool properties = true)
    {
        if(properties) return typeof(T).GetProperties().Select(p => p.Name);
        else return typeof(T).GetProperties().Select(p => "'" + p.GetValue(entity) + "'");
    }
}
