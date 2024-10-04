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

    public Task<int> InsertAsync(Meal entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(Meal entity)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(string name)
    {
        throw new NotImplementedException();
    }
}
