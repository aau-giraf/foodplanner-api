using System.Reflection.Metadata;
using Dapper;
using FoodplannerModels.Image;
using FoodplannerModels.Tags;
using Microsoft.AspNetCore.Identity;

namespace FoodplannerDataAccessSql.Tags;

public class TagsRepository
{
      private readonly CorePostgreSQLConnectionFactory _connectionFactory;

        public TagsRepository(CorePostgreSQLConnectionFactory connectionFactory)
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
            ON i.ingredient_id = t.ingredient_id
        INNER JOIN categories c
            ON t.category_id = c.id
        WHERE c.name = ANY(@Tags)
            AND i.image_id IS NOT NULL
        GROUP BY i.ingredient_id, i.image_id
        HAVING COUNT(DISTINCT c.name) = CARDINALITY(@Tags);
        """;
        using (var connection = _connectionFactory.Create())
        {
            connection.Open();
            var result = await connection.QueryAsync<int>(sql, new {Tags = tags});
            return result;
        }
    }
}