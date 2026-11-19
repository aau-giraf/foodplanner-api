using Dapper;
using FoodplannerModels.Tags;

// Adds Category Repository to the FoodplannerDataAccessSql.Tags namespace
namespace FoodplannerDataAccessSql.Tags;

// The repository for the Category class
// Implements interface CategoryRepository
public class CategoryRepository : ICategoryRepository
{
      private readonly CorePostgreSQLConnectionFactory _connectionFactory;

        // Constructor for the connection factory class
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

    // Method that checks if category already exists
    // Returns true if category already exists, returns false if category does not exists
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

    // Method to insert a new category into categories table 
    // Returns category ID
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

    // Method for retrieving category by ID
    // Returns category with specific ID
    public async Task<CategoryDTO> GetByIdAsync(int id)
    {
        var sql = "SELECT id, name, main FROM categories WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<CategoryDTO>(sql, new {Id = id});
            return result;
        }
       
    }

    // Method to update category from categories table based on ID
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

    // Method for deleting a category from categories table based on ID
    // Uses a transaction to roll back the deletion if an error occurs
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

