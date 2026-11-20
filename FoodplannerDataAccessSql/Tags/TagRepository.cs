using Dapper;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public class TagRepository : ITagRepository
{
      private readonly PostgreSQLConnectionFactory _connectionFactory;

        // Constructor for the connection factory class
        public TagRepository(PostgreSQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

    // retrieve image ids that match all the tags in given list (e.g "fruit" AND "sliced")
    public async Task<IEnumerable<int>> GetImageIdsFromTags(List<string> tags)
    {
        var sql = """
        SELECT DISTINCT i.image_id
        FROM ingredients i
        INNER JOIN tags t
            ON i.id = t.ingredient_id
        INNER JOIN categories c
            ON t.category_id = c.id
        WHERE c.name = ANY(@Tags)
            AND i.image_id IS NOT NULL
        GROUP BY i.id, i.image_id
        HAVING COUNT(DISTINCT c.name) = CARDINALITY(@Tags);
        """;
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<int>(sql, new {Tags = tags});
            return result;
        }
    }

    // Method that checks if category already exists
    // Returns true if category already exists, returns false if category does not exists
    public async Task<bool> TagExistsAsync(int Category_id, int Ingredient_id)
    {
        var sql = "SELECT COUNT(1) FROM tags WHERE category_id = @Category_Id AND ingredients_id = @Ingredient_id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var count = await connection.ExecuteScalarAsync<int>(sql, new {Category_id = Category_id, Ingredient_id = Ingredient_id});
            return count > 0;
        }
    }

    // Method to retrieve all tags from the database
    public Task<IEnumerable<TagDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM tags";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = connection.QueryAsync<TagDTO>(sql);
            return result;
        }

    }

    // Method to insert a new tag into tags table 
    // Returns tag ID
    public Task<int> InsertAsync(TagDTO entity)
    {
        var sql = "INSERT INTO tags (category_id, ingredient_id) VALUES (@Category_id, @Ingredient_id) RETURNING id";

        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = connection.QuerySingleAsync<int>(sql, new
            {
                category_id = entity.Category_id,
                ingredient_id = entity.Ingredient_id
            });
            return result;
        }
    }

    // Method for retrieving tag by ID
    // Returns tag with specific ID
    public async Task<TagDTO> GetByIdAsync(int id)
    {
        var sql = "SELECT id, name, main FROM categories WHERE id = @Id";
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryFirstOrDefaultAsync<TagDTO>(sql, new {Id = id});
            return result;
        }
       
    }

    // Method to update tag from tags table based on ID
    public Task<int> UpdateAsync(TagDTO entity)
    {
        var sql = "UPDATE tags SET category_id = @Category_id, ingredient_id = @Ingredient_id WHERE id = @Id";
        using (var connection = _connectionFactory.Create()) 
        {
            connection.Open();
            var result = connection.Execute(sql, new
            {
                entity.Category_id,
                entity.Ingredient_id,
            });
            return Task.FromResult(result);
        }
    }

    // Method for deleting a tag from tags table based on ID
    // Uses a transaction to roll back the deletion if an error occurs
    public Task<int> DeleteAsync(int id)
    {
        var deleteCategorySql = "DELETION FROM tags WHERE id = @id";

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