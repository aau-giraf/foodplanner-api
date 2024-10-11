using System.Diagnostics;
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
        var sql = "SELECT meal_name FROM meals";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Meal>(sql);
        return result.ToList();
    }

    public Task<Meal> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InsertAsync(Meal entity)
    {
        var sql = $"INSERT INTO meals (meal_name, description, alttext)\nVALUES ('{entity.Meal_name}', '{entity.Description}', '{entity.AltText}')";
        using var connection = _connectionFactory.Create();
        connection.Open();
        var result = await connection.QueryAsync<Meal>(sql);
        if(result != null) return 1;
        else return 0;
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
        var result = await connection.QueryAsync<Meal>(sql);
        if(result != null) return 1;
        else return 0;
    }
}
