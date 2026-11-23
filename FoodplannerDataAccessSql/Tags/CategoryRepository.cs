using Dapper;
using FoodplannerModels.Tags;

// Adds Category Repository to the FoodplannerDataAccessSql.Tags namespace
namespace FoodplannerDataAccessSql.Tags;

// The repository for the Category class
// Implements interface CategoryRepository
public class CategoryRepository(IDapperExecutor executor) : ICategoryRepository
{

    // Get all categories from the database
    public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM categories";

        return await executor.QueryAsync<CategoryDTO>(sql);

    }

    // Method that checks if category already exists
    // Returns true if category already exists, returns false if category does not exists
    public async Task<bool> CategoryExistsAsync(string name)
    {
        var sql = "SELECT COUNT(1) FROM categories WHERE name = @Name";
        var count =  await executor.QuerySingleAsync<int>(sql, new {Name = name});
        return count > 0;
    }

    // Method to insert a new category into categories table 
    // Returns category ID
    public async Task<int> InsertAsync(CategoryDTO entity)
    {
        var sql = "INSERT INTO categories (name, main) VALUES (@Name, @Main) RETURNING id";

        return await executor.QuerySingleAsync<int>(sql, new {name = entity.Name, main = entity.Main});
    }

    // Method for retrieving category by ID
    // Returns category with specific ID
    public async Task<CategoryDTO?> GetByIdAsync(int id)
    {
        var sql = "SELECT id, name, main FROM categories WHERE id = @Id";
        return await executor.QuerySingleOrDefaultAsync<CategoryDTO>(sql, new {Id = id});       
    }

    // Method to update category from categories table based on ID
    public async Task<int> UpdateAsync(CategoryDTO entity)
    {
        var sql = "UPDATE categories SET name = @Name, main = @Main WHERE id = @Id";
        return await executor.ExecuteAsync(sql, new
        {
                entity.Id,
                entity.Name,
                entity.Main
        });
    }

    // Method for deleting a category from categories table based on ID
    // Uses a transaction to roll back the deletion if an error occurs
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETE FROM categories WHERE id = @Id";
        return await executor.ExecuteAsync(sql, new {Id = id});
    }
}

