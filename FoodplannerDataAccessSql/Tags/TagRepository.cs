using Dapper;
using FoodplannerModels.Tags;

namespace FoodplannerDataAccessSql.Tags;

public class TagRepository(IDapperExecutor executor) : ITagRepository
{
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
        return await executor.QueryAsync<int>(sql, new { Tags = tags });
    }

    // Method that checks if category already exists
    // Returns true if category already exists, returns false if category does not exists
    public async Task<bool> TagExistsAsync(int category_id, int ingredient_id)
    {
        var sql = "SELECT COUNT(1) FROM tags WHERE category_id = @Category_Id AND ingredients_id = @Ingredient_id";
        var result = await executor.QueryAsync<int>(sql, new {Category_id = category_id, Ingredient_id = ingredient_id});
        int[] array = result.ToArray();
        
        return array[0] > 0;
    }

    // Method to retrieve all tags from the database
    public async Task<IEnumerable<TagDTO>> GetAllAsync()
    {
        var sql = "SELECT * FROM tags";
        return await executor.QueryAsync<TagDTO>(sql);
    }

    // Method to insert a new tag into tags table 
    // Returns tag ID
    public async Task<int> InsertAsync(TagDTO entity)
    {
        var sql = "INSERT INTO tags (category_id, ingredient_id) VALUES (@Category_id, @Ingredient_id) RETURNING id";
        return await executor.QuerySingleAsync<int>(sql);
    }

    // Method for retrieving tag by ID
    // Returns tag with specific ID
    public async Task<TagDTO?> GetByIdAsync(int id)
    {
        var sql = "SELECT id, name, main FROM categories WHERE id = @Id";
        return await executor.QuerySingleAsync<TagDTO>(sql);       
    }

    // Method to update tag from tags table based on ID
    public async Task<int> UpdateAsync(TagDTO entity)
    {
        var sql = "UPDATE tags SET category_id = @Category_id, ingredient_id = @Ingredient_id WHERE id = @Id";
        return await executor.QuerySingleAsync<int>(sql, new 
            {Category_id = entity.Category_id, Ingredient_id = entity.Ingredient_id}); 
    }

    // Method for deleting a tag from tags table based on ID
    // Uses a transaction to roll back the deletion if an error occurs
    public async Task<int> DeleteAsync(int id)
    {
        var sql = "DELETION FROM tags WHERE id = @id";

        return await executor.ExecuteAsync(sql, new {Id = id});
    }

}