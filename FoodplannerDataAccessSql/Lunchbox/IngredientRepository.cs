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
        var sql = "SELECT name FROM ingredients";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Ingredient>(sql);
        return result.ToList();
    }

    public async Task<Ingredient> GetByNameAsync(string name)
    {
        var sql = $"SELECT name FROM ingredients WHERE name = '{name}' GROUP BY name";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Meal>(sql);
        return (Ingredient)result;
    }

    public async Task<int> InsertAsync(Ingredient entity)
    {
        var sql = "INSERT INTO ingredients (name)\n";
        sql += $"VALUES ('{entity.Name}')";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Ingredient>(sql);
        if(result != null) return 1;
        else return 0;
    }
}
