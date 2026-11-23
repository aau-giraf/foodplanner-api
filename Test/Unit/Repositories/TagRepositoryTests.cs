using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Lunchbox;
using FoodplannerModels.Tags;
using Moq;
using Xunit;

namespace Test.Unit.RepositoryTests;

public class TagRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly TagRepository repository;

    public TagRepositoryTests()
    {
        repository = new TagRepository(mockExecutor.Object);
    }

    private static bool HasParameter(object? parameters, string name, object? expectedValue)
    {
        if (parameters == null)
            return false;

        var property = parameters.GetType().GetProperty(name);

        if (property == null)
            return false;

        return Equals(property.GetValue(parameters), expectedValue);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetImageIdsFromTags_Successs()
    {
        // Arrange 
        // Create food image ids 
        var foodImageIds = new List<int> {2,4}; 
        
        // Categories
        var categories = new List<string> { "fruit", "sliced"}; 

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

        // When call with sql string, return food image ids
        mockExecutor.Setup(e => e.QueryAsync<int>(sql, It.IsAny<object?>()))
            .ReturnsAsync(foodImageIds);

        // Act
        // call repository method
        var result = await repository.GetImageIdsFromTags(categories);

        // Assert 
        // Convert to list 
        var list = result.ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(3, list[1]);

         // Verifies the sql string is called once and with parameter null
        mockExecutor.Verify(e => e.QueryAsync<int>(sql, null), Times.Once);        
    }
}