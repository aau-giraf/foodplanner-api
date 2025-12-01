using Dapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerDataAccessSql.Lunchbox;

/// <summary>
/// Repository implementation for SubIngredientRelation data access
/// </summary>
public class SubIngredientRelationRepository : ISubIngredientRelationRepository
{
    private readonly PostgreSQLConnectionFactory _connectionFactory;

    public SubIngredientRelationRepository(PostgreSQLConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<SubIngredientRelation>> GetAllAsync()
    {
        var sql = "SELECT * FROM subingredient_relation";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<SubIngredientRelation>(sql);
        }
    }

    public async Task<IEnumerable<SubIngredientRelation>> GetAllByIngredientIdAsync(int ingredientId)
    {
        var sql = "SELECT * FROM subingredient_relation WHERE ingredient_id = @IngredientId ORDER BY order_number";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QueryAsync<SubIngredientRelation>(sql, new { IngredientId = ingredientId });
        }
    }

    public async Task<SubIngredientRelation> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM subingredient_relation WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleOrDefaultAsync<SubIngredientRelation>(sql, new { Id = id });
            return result;
        }
    }

    public async Task<int> InsertAsync(int ingredientId, int subingredientId)
    {
        var sql = "INSERT INTO subingredient_relation (ingredient_id, subingredient_id) VALUES (@IngredientId, @SubingredientId) RETURNING id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.QuerySingleAsync<int>(sql, new
            {
                IngredientId = ingredientId,
                SubingredientId = subingredientId
            });
        }
    }

    public async Task<int> UpdateAsync(SubIngredientRelation entity, int id)
    {
        var sql = "UPDATE subingredient_relation SET ingredient_id = @IngredientId, subingredient_id = @SubingredientId WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new
            {
                Id = id,
                IngredientId = entity.Ingredient_id,
                SubingredientId = entity.Subingredient_id
            });
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM subingredient_relation WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

    public async Task<bool> UpdateOrderAsync(int id, int order)
    {
        var sql = "UPDATE subingredient_relation SET order_number = @Order WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            return await connection.ExecuteAsync(sql, new { Id = id, Order = order }) > 0;
        }
    }
}