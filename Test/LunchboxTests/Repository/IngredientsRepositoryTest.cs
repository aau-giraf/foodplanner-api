using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Abstractions;
using Moq;
using SQLitePCL;
using Xunit;



public class IngredientsRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly IngredientsRepository repository;

    public IngredientsRepositoryTests()
    {
        repository = new IngredientsRepository(mockExecutor.Object);
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
    public async Task GetAllAsync_Success()
    {
        // Arrange
        // Create ingredient objects
        var ingredients = new List<IngredientDTO>
        {
            new() { Id = 1, Name = "Cheese", User_id = 1 },
            new() { Id = 2, Name = "Tomato", User_id = 2 }
        };

        const string sql = "SELECT * FROM ingredients";

        // When call with sql string, return ingredient objects
        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
            sql, It.IsAny<object?>())).ReturnsAsync(ingredients);

        // Act
        // Call the repository method
        var result = await repository.GetAllAsync();

        // Assert
        // Convert result into a list 
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.Equal(1, resultList[0].Id);
        Assert.Equal("Cheese", resultList[0].Name);
        Assert.Equal(1, resultList[0].User_id);
        Assert.Equal(2, resultList[1].Id);
        Assert.Equal("Tomato", resultList[1].Name);
        Assert.Equal(2, resultList[1].User_id);

        // Verifies the sql string is called once and with parameter null
        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(sql, null), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserAsync_Success()
    {
        // Arrange
        // Random seed for the id 
        var rnd = new Random(); 
        int id = rnd.Next();

        // ingredient objects
        var ingredients = new List<IngredientDTO>
        {
            new() { Id = 1, Name = "Cheese", User_id = id },
            new() { Id = 2, Name = "Tomato", User_id = id }
        };

        const string sql = "SELECT * FROM ingredients WHERE user_id = @Id";

        // When call the sql string, return the ingredient objects
        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
            sql, It.IsAny<object?>())).ReturnsAsync(ingredients);

        // Act
        var result = await repository.GetAllByUserAsync(id);

        // Assert
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.Equal(1, resultList[0].Id);
        Assert.Equal("Cheese", resultList[0].Name);
        Assert.Equal(id, resultList[0].User_id);
        Assert.Equal(2, resultList[1].Id);
        Assert.Equal("Tomato", resultList[1].Name);
        Assert.Equal(id, resultList[1].User_id);

        // Verifies that the sql is called once, 
        // and the parameter object is not null and has property "id" that has the same value as id
        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(sql, It.Is<object>(p => p != null && 
            Equals(p.GetType().GetProperty("Id")!.GetValue(p), id))), Times.Once);
    }


    [Fact]
    public async Task GetByIdAsync_Success()
    {
        // Arrange
        var rnd = new Random(); 
        int id = rnd.Next();

        var ingredient = new IngredientDTO { Id = id, Name = "Cheese", User_id = 3 };

        const string sql = "SELECT * FROM ingredients WHERE id = @Id";

        mockExecutor
            .Setup(e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
                sql, It.IsAny<object?>()))
            .ReturnsAsync(ingredient);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Cheese", result.Name);
        Assert.Equal(3, result.User_id);
        
        // Verify that the query is called with an object that is not null 
        // and the object has a property "id" with a value equal to the id
        mockExecutor.Verify(
        e => e.QuerySingleOrDefaultAsync<IngredientDTO>(
            sql,
            It.Is<object>(p => p != null &&
            Equals(p.GetType().GetProperty("Id")!.GetValue(p), id))),
        Times.Once);
    }

    [Fact]
    public async Task InsertAsync_Success()
    {
        // Arrange
        var rnd = new Random(); 
        int createId = rnd.Next();
        var ingredient = new IngredientDTO { Id = 0, Name = "Cheese", User_id = 1, Image_id = null };

        const string sql = "INSERT INTO ingredients (name, user_id, image_id) VALUES (@Name, @UserId, @ImageId) RETURNING id";

        mockExecutor.Setup(e => e.QuerySingleAsync<int>(sql, 
        It.IsAny<object?>())).ReturnsAsync(createId);

        // Act
        var result = await repository.InsertAsync(ingredient);

        // Assert
        // Id should be same as result 
        Assert.Equal(createId, result); 

        // Verify that the query is called with an object 
        // and the object has a properties matching properties from the object
        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "Name", ingredient.Name) &&
                HasParameter(p, "UserId", ingredient.User_id) &&
                HasParameter(p, "ImageId", DBNull.Value))), //Image is a special type of db null value
        Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        // Arrange
        var rnd = new Random(); 
        int id = rnd.Next();

        var ingredient = new IngredientDTO { Id = id, Name = "Cheese", User_id = 5, Image_id = null };

        const string sql = "UPDATE ingredients SET name = @Name, user_id = @UserId, image_id = @ImageId WHERE id = @Id";

        // When call sql string, return the id 
       mockExecutor.Setup(e => e.QuerySingleAsync<int>(sql, 
        It.IsAny<object?>())).ReturnsAsync(1);

        // Act
        var result = await repository.UpdateAsync(ingredient);

        // Assert
        // result should be 1, since only one entry updated
        Assert.Equal(1, result); 

        // Verify that the query is called with an object that is not null 
        // and the object has a properties matching properties from the object
        // Verify that the query is called with an object 
        // and the object has a properties matching properties from the object
        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "Name", ingredient.Name) &&
                HasParameter(p, "UserId", ingredient.User_id) &&
                HasParameter(p, "ImageId", DBNull.Value))), //Image is a special type of db null value
        Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        // Arrange
        var rnd = new Random(); 
        int id = rnd.Next(); 

        var ingredient = new IngredientDTO { Id = id, Name = "Cheese", User_id = 1, Image_id = null };

        const string sql = "DELETE FROM ingredients WHERE id = @Id";

        // When call sql string, return the id 
        mockExecutor.Setup(e => e.QuerySingleAsync<int>(sql, 
        It.IsAny<object?>())).ReturnsAsync(1);

        // Act
        var result = await repository.DeleteAsync(id);

        // Assert
        // result should be 1, since only one entry updated
        Assert.Equal(1, result); 

        // Verify that the query is called with an object 
        // and the object has a properties matching properties from the object
        mockExecutor.Verify(e => e.QuerySingleAsync<int>(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "Name", ingredient.Name) &&
                HasParameter(p, "UserId", ingredient.User_id) &&
                HasParameter(p, "ImageId", DBNull.Value))), //Image is a special type of db null value
        Times.Once);
    }
}

