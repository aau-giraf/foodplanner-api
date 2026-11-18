using System.Reflection.Metadata;
using Dapper;
using FoodplannerModels.Tags;
using Microsoft.AspNetCore.Identity;

namespace FoodplannerDataAccessSql.Tags;

public class CategoriesRepository 
{
      private readonly CorePostgreSQLConnectionFactory _connectionFactory;

        public CategoriesRepository(CorePostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    // Get all categories from the database
    public async Task<IEnumerable<Categories>> GetAllAsync()
    {
        var sql = "SELECT * FROM categories";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<Categories>(sql);
            return result;
        }

    }

    public async Task<Categories> InsertAsync(Categories entity)
    {
        var sql = "INSERT INTO categories (name, main) VALUES (@Name, @Main)";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QuerySingleAsync<Categories>(sql, new
            {
                name = entity.Name,
                main = entity.Main
            });
            return result;
        }

        
    }
}

