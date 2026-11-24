using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Tags;
using FoodplannerModels.Tags;
using Moq;

// Adds CategoryrepositoryTests to the Test.Unit.RepositoryTests namespace
namespace Test.Unit.RepositoryTests;

// Tests for Category Repository class 
public class CategoryRepositoryTests
{
    // Mocks the dapper executor 
    private readonly Mock<IDapperExecutor> MockExecutor = new();

    private readonly CategoryRepository Repository;

    // Constructor for the repository with mock executor
    public CategoryRepositoryTests()
    {
        Repository = new CategoryRepository(MockExecutor.Object);
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
    public async Task CreateCategory_Success()
    {
       // Arrange 
       // Creates a random category ID
       var Rnd = new Random();
       int CategoryId = Rnd.Next();

       // Creates a main category called "frugt"
       var Category = new CategoryDTO
       {
           Name = "frugt",
           Main = true
       };


        // SQL query that inserts a new category 
        const string sql = "INSERT INTO categories (name, main) VALUES (@Name, @Main) RETURNING id";

        // Returns CategoryID when SQL query is called
        MockExecutor.Setup(e => e.QuerySingleAsync<int>(sql, It.IsAny<object?>())).ReturnsAsync(CategoryId);


        // Act
        // Calls repository method
        var Result = await Repository.InsertAsync(Category);

        // Assert
        // Checks that the returned ID matches with the generated ID
        Assert.Equal(CategoryId, Result);

        // Verify that the correct SQL and parameters were passed 
        MockExecutor.Verify(e => e.QuerySingleAsync<int>(sql, It.Is<object?>(p => HasParameter(p, "Name", Category.Name) && HasParameter(p, "Main", Category.Main))), Times.Once);
     

        
    }
}