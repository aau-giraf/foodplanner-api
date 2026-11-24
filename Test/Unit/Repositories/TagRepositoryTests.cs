using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;
using Moq;

// Adds Tag Repository to the Test.Unit.RepositoryTests namespace
namespace Test.Unit.RepositoryTests;

// Tests for Tag Repository class
public class TagRepositoryTests
{
    // Mocks the dapper executor 
    private readonly Mock<IDapperExecutor> MockExecutor = new();
    private readonly TagRepository Repository;

    // Constructor for the repository with mock executor
    public TagRepositoryTests()
    {
        Repository = new TagRepository(MockExecutor.Object);
    }

     // Helper method 
    private static bool HasParameter(object? Parameters, string Name, object? ExpectedValue)
    {
        if (Parameters == null)
            return false;

        var Property = Parameters.GetType().GetProperty(Name);

        if (Property == null)
            return false;

        return Equals(Property.GetValue(Parameters), ExpectedValue);
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
        MockExecutor.Setup(e => e.QueryAsync<int>(sql, It.IsAny<object?>()))
            .ReturnsAsync(foodImageIds);

        // Act
        // Call repository method
        var result = await Repository.GetImageIdsFromTagsAsync(categories);

        // Assert 
        // Convert to list 
        var list = result.ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(4, list[1]);

        // Verifies the sql string is called once and with parameter null
        MockExecutor.Verify(e =>e.QueryAsync<int>(sql, It.IsAny<object>()),Times.Once);       
    }


    [Fact]
    [Trait("Tags", "Unit")]
    public async Task CreateTag_Success()
    {
       // Arrange 
       // Creates a random tagID, IngredientID and CategoryID
       var Rnd = new Random();
       int TagId = Rnd.Next();
       int Ingredient_id = Rnd.Next();
       int Category_id = Rnd.Next();

       // Creates a Tag based on the ingredientID and CategoryID
       var Tag = new TagDTO
       {
           Ingredient_id = Ingredient_id, 
           Category_id = Category_id
       };

        // SQL query that inserts a new tag
        const string sql = "INSERT INTO tags (ingredient_id, category_id) VALUES (@Ingredient_id, @Category_id) RETURNING id";

        // Returns TagID when SQL query is called
        MockExecutor.Setup(e => e.QuerySingleAsync<int>(sql, It.IsAny<object?>())).ReturnsAsync(TagId);

        // Act
        // Calls repository method
        var Result = await Repository.InsertAsync(Tag);

        // Assert
        // Checks that the returned ID matches with the generated ID
        Assert.Equal(TagId, Result);

        // Verify that the correct SQL and parameters were passed 
        MockExecutor.Verify(e => e.QuerySingleAsync<int>(sql, It.Is<object?>(p => HasParameter(p, "Ingredient_id", Tag.Ingredient_id) && HasParameter(p, "Category_id", Tag.Category_id))), Times.Once);
        
    }
}