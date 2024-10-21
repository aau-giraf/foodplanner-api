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

    public async Task<Ingredient> GetByIdAsync(int id)
    {
        var sql = $"SELECT * FROM ingredients WHERE id = '{id}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QuerySingleOrDefaultAsync<Ingredient>(sql, new { Id = id });
        if (result == null) return null;
        else return result;
    }

    public async Task<int> InsertAsync(Ingredient entity)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"INSERT INTO ingredients (name, user_ref, image_ref)\n";
        sql += $"VALUES ('{entity.Name}', '{entity.User_ref}', '{entity.Image_ref}')";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> UpdateAsync(Ingredient entity, int id)
    {
        using var connection = _connectionFactory.Create();
        connection.Open();
        var sql = $"UPDATE ingredients\n";
        sql += $"SET name = '{entity.Name}', user_ref = '{entity.User_ref}', image_ref = '{entity.Image_ref}'";
        sql += $"WHERE id = '{id}'";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = $"DELETE FROM ingredients WHERE id = '{id}'";
        using var connection = _connectionFactory.Create();
        connection.Open();
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
