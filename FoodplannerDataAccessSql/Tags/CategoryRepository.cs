using Dapper;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public class CategoryRepository : ICategoryRepository
{
      private readonly CorePostgreSQLConnectionFactory _connectionFactory;

        public CategoryRepository(CorePostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
    // Get all categories from the database
    public Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM categories";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = connection.QueryAsync<CategoryDTO>(sql);
            return result;
        }

    }

    public async Task<bool> CategoryExistsAsync(string name)
    {
        var sql = "SELECT COUNT(1) FROM categories WHERE name = @Name";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name});
            return count > 0;
        }
    }

    public Task<int> InsertAsync(CategoryDTO entity)
    {
        var sql = "INSERT INTO categories (name, main) VALUES (@Name, @Main) RETURNING id";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = connection.QuerySingleAsync<int>(sql, new
            {
                name = entity.Name,
                main = entity.Main
            });
            return result;
        }

        
    }

    public async Task<CategoryDTO> GetByIdAsync(int id)
    {
        return null;
    }

    public Task<int> UpdateAsync(CategoryDTO entity)
    {
        var sql = "UPDATE categories SET name = @Name, main = @Main WHERE id = @Id";
        using (var connection = _connectionFactory.Create()) 
        {
            connection.Open();
            var result = connection.Execute(sql, new
            {
                entity.Id,
                entity.Name,
                entity.Main
            });
            return Task.FromResult(result);
        }
    }

    public Task<int> DeleteAsync(int id)
    {
        var deleteCategorySql = "DELETION FROM categories WHERE id = @id";

            using (var connection = _connectionFactory.Create())
        {
            using var transaction = connection.BeginTransaction();
                try
                {
                    var result = connection.ExecuteAsync(deleteCategorySql, new { Id = id }, transaction);

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }           
            }
    }
}

