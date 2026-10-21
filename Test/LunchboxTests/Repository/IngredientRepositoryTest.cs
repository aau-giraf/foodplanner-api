using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;
using Xunit;

public class IngredientsRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly IngredientsRepository repository;

    public IngredientsRepositoryTests()
    {
        repository = new IngredientsRepository(mockExecutor.Object);
    }

    [Fact]
    public async Task GetByIdAsync_Success()
    {
        // Arrange
        int id = 1;
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
    }

    [Fact]
    public async Task GetAllAsync_Success()
    {
        // Arrange
        var ingredients = new List<IngredientDTO>
        {
            new() { Id = 1, Name = "Cheese", User_id = 1 },
            new() { Id = 2, Name = "Tomato", User_id = 2 }
        };

        const string sql = "SELECT * FROM ingredients";

        mockExecutor.Setup(e => e.QueryAsync<IngredientDTO>(
            sql, It.IsAny<object?>())).ReturnsAsync(ingredients);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);
        Assert.Equal(1, resultList[0].Id);
        Assert.Equal("Cheese", resultList[0].Name);
        Assert.Equal(1, resultList[0].User_id);
        Assert.Equal(2, resultList[1].Id);
        Assert.Equal("Tomato", resultList[1].Name);
        Assert.Equal(2, resultList[1].User_id);

        mockExecutor.Verify(e => e.QueryAsync<IngredientDTO>(sql, null), Times.Once);
    }
}

