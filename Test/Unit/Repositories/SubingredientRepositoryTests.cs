using FoodplannerDataAccessSql;
using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;
using Xunit;

public class SubIngredientsRepositoryTests
{
    private readonly Mock<IDapperExecutor> mockExecutor = new();
    private readonly SubIngredientRepository repository;

    public SubIngredientsRepositoryTests()
    {
        repository = new SubIngredientRepository(mockExecutor.Object);
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
        var rnd = new Random();
        int parent_id1 = rnd.Next();
        int parent_id2 = rnd.Next();
        int child_id1 = rnd.Next();
        int child_id2 = rnd.Next();

        // Create subingredient objects
        var subingredients = new List<SubIngredientDTO>
        {
            new() { Parent_id = parent_id1, Child_id = child_id1, order_no = 1 },
            new() { Parent_id = parent_id2, Child_id = child_id2, order_no = 2 }
        };

        const string sql = "SELECT * FROM subingredient";

        // When called with SQL string, return subingredient objects
        mockExecutor.Setup(e => e.QueryAsync<SubIngredientDTO>(
            sql, It.IsAny<object?>())).ReturnsAsync(subingredients);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);

        Assert.Equal(parent_id1, resultList[0].Parent_id);
        Assert.Equal(child_id1, resultList[0].Child_id);
        Assert.Equal(1, resultList[0].order_no);

        Assert.Equal(parent_id2, resultList[1].Parent_id);
        Assert.Equal(child_id2, resultList[1].Child_id);
        Assert.Equal(2, resultList[1].order_no);

        // Verify the SQL string is called once with null parameters
        mockExecutor.Verify(e => e.QueryAsync<SubIngredientDTO>(
            sql, null), Times.Once);
    }

    [Fact]
    public async Task GetAllByParentIdAsync_Success()
    {
        // Arrange
        var rnd = new Random();
        int parent_id = rnd.Next();
        int child_id1 = rnd.Next();
        int child_id2 = rnd.Next();

        // Create subingredient objects
        var subingredients = new List<SubIngredientDTO>
        {
            new() { Parent_id = parent_id, Child_id = child_id1, order_no = 1 },
            new() { Parent_id = parent_id, Child_id = child_id2, order_no = 2 }
        };

        const string sql = "SELECT * FROM subingredient WHERE parent_id = @Id ORDER BY order_no";

        // When called with SQL string, return subingredient objects
        mockExecutor.Setup(e => e.QueryAsync<SubIngredientDTO>(
            sql, It.IsAny<object?>())).ReturnsAsync(subingredients);

        // Act
        var result = await repository.GetAllByParentIdAsync(parent_id);

        // Assert
        var resultList = result.ToList();

        Assert.Equal(2, resultList.Count);

        Assert.Equal(parent_id, resultList[0].Parent_id);
        Assert.Equal(child_id1, resultList[0].Child_id);
        Assert.Equal(1, resultList[0].order_no);

        Assert.Equal(parent_id, resultList[1].Parent_id);
        Assert.Equal(child_id2, resultList[1].Child_id);
        Assert.Equal(2, resultList[1].order_no);

        // Verify that SQL was called with the correct parent ID
        mockExecutor.Verify(e => e.QueryAsync<SubIngredientDTO>(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "Id", parent_id))),
            Times.Once);
    }

    [Fact]
    public async Task InsertAsync_Success()
    {
        // Arrange
        var rnd = new Random();
        int parent_id = rnd.Next();
        int child_id = rnd.Next();

        var subingredient = new SubIngredientDTO
        {
            Parent_id = parent_id,
            Child_id = child_id,
            order_no = 1
        };

        const string sql = "INSERT INTO subingredient (parent_id, child_id, order_no) VALUES (@ParentId, @ChildId, @OrderNumber)";

        // When SQL is executed, return 1 affected row
        mockExecutor.Setup(e => e.ExecuteAsync(
            sql, It.IsAny<object?>())).ReturnsAsync(1);

        // Act
        var result = await repository.InsertAsync(subingredient);

        // Assert
        Assert.Equal(1, result);

        // Verify the correct parameters were passed
        mockExecutor.Verify(e => e.ExecuteAsync(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "ParentId", subingredient.Parent_id) &&
                HasParameter(p, "ChildId", subingredient.Child_id) &&
                HasParameter(p, "OrderNumber", subingredient.order_no))),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Success()
    {
        // Arrange
        var rnd = new Random();

        int parent_id_new = rnd.Next();
        int child_id_new = rnd.Next();
        int parent_id_old = rnd.Next();
        int child_id_old = rnd.Next();

        var subingredient_new = new SubIngredientDTO
        {
            Parent_id = parent_id_new,
            Child_id = child_id_new,
            order_no = 1
        };

        var subingredient_old = new SubIngredientDTO
        {
            Parent_id = parent_id_old,
            Child_id = child_id_old,
            order_no = 2
        };

        const string sql = "UPDATE subingredient SET parent_id = @NewParentId, child_id = @NewChildId, order_no = @NewOrderNumber WHERE parent_id = @OldParentId AND child_id = @OldChildId";

        // When SQL is executed, return 1 affected row
        mockExecutor.Setup(e => e.ExecuteAsync(
            sql, It.IsAny<object?>())).ReturnsAsync(1);

        // Act
        var result = await repository.UpdateAsync(subingredient_old, subingredient_new);

        // Assert
        Assert.Equal(1, result);

        // Verify both old and new values were passed correctly
        mockExecutor.Verify(e => e.ExecuteAsync(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "NewParentId", subingredient_new.Parent_id) &&
                HasParameter(p, "NewChildId", subingredient_new.Child_id) &&
                HasParameter(p, "NewOrderNumber", subingredient_new.order_no) &&
                HasParameter(p, "OldParentId", subingredient_old.Parent_id) &&
                HasParameter(p, "OldChildId", subingredient_old.Child_id))),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Success()
    {
        // Arrange
        var rnd = new Random();

        int parent_id = rnd.Next();
        int child_id = rnd.Next();

        var subingredient = new SubIngredientDTO{Parent_id = parent_id, Child_id = child_id, order_no = 1};

        const string sql = "DELETE FROM subingredient WHERE parent_id = @ParentId AND child_id = @ChildId";

        // When SQL is executed, return 1 affected row
        mockExecutor.Setup(e => e.ExecuteAsync(
            sql, It.IsAny<object?>())).ReturnsAsync(1);

        // Act
        var result = await repository.DeleteAsync(subingredient);

        // Assert
        Assert.Equal(1, result);

        // Verify that the correct parameters were passed
        mockExecutor.Verify(e => e.ExecuteAsync(
            sql,
            It.Is<object?>(p =>
                HasParameter(p, "ParentId", subingredient.Parent_id) &&
                HasParameter(p, "ChildId", subingredient.Child_id))),
            Times.Once);
    }

}

