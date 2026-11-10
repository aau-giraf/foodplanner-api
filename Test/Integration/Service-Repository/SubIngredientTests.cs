using FoodplannerDataAccessSql.Lunchbox;
using FoodplannerDataAccessSql;
using FoodplannerServices.Lunchbox;
using FoodplannerModels.Lunchbox;
using Moq;

namespace Test.Integration.Services;

public class SubIngredientTests
{
    private readonly Mock<IDapperExecutor> mockDapperExecutor = new();
    private readonly SubIngredientRepository subIngredientRepository;
    private readonly SubIngredientService service;
    public SubIngredientTests()
    {
        subIngredientRepository = new SubIngredientRepository(mockDapperExecutor.Object);
        service = new SubIngredientService(subIngredientRepository);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllSubIngredientsAsync()
    {
        // Arrange
        var expected = new List<SubIngredientDTO>
        {
            new() { Parent_id = 1, Child_id = 1, order_no = 1 },
            new() { Parent_id = 1, Child_id = 2, order_no = 2 },
            new() { Parent_id = 2, Child_id = 3, order_no = 1 }
        };
        mockDapperExecutor.Setup(x => x.QueryAsync<SubIngredientDTO>("SELECT * FROM subingredient")).ReturnsAsync(expected);

        // Act
        var result = await service.GetAllSubIngredientsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Count());

        Assert.Collection(result,
            item =>
            {
                Assert.Equal(1, item.Parent_id);
                Assert.Equal(1, item.Child_id);
                Assert.Equal(1, item.order_no);
            },
            item =>
            {
                Assert.Equal(1, item.Parent_id);
                Assert.Equal(2, item.Child_id);
                Assert.Equal(2, item.order_no);
            },
            item =>
            {
                Assert.Equal(2, item.Parent_id);
                Assert.Equal(3, item.Child_id);
                Assert.Equal(1, item.order_no);
            });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAllSubIngredientsByParentIdAsync()
    {
        // Arrange
        int parentId = 1;
        var expected = new List<SubIngredientDTO>
        {
            new() { Parent_id = parentId, Child_id = 1, order_no = 1 },
            new() { Parent_id = parentId, Child_id = 2, order_no = 2 }
        };
        mockDapperExecutor.Setup(x => x.QueryAsync<SubIngredientDTO>(
            "SELECT * FROM subingredient WHERE parent_id = @Id ORDER BY order_no",
            It.Is<object>(param => (int)param.GetType().GetProperty("Id")!.GetValue(param)! == parentId)))
            .ReturnsAsync(expected);

        // Act
        var result = await service.GetAllSubIngredientsByParentIdAsync(parentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Count());
        Assert.Collection(result,
            item =>
            {
                Assert.Equal(parentId, item.Parent_id);
                Assert.Equal(1, item.Child_id);
                Assert.Equal(1, item.order_no);
            },
            item =>
            {
                Assert.Equal(parentId, item.Parent_id);
                Assert.Equal(2, item.Child_id);
                Assert.Equal(2, item.order_no);
            });
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateSubIngredientAsync()
    {
        // Arrange
        var newSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 3, order_no = 3 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "INSERT INTO subingredient (parent_id, child_id, order_no) VALUES (@ParentId, @ChildId, @OrderNumber)",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("ParentId")!.GetValue(param)! == newSubIngredient.Parent_id &&
                (int)param.GetType().GetProperty("ChildId")!.GetValue(param)! == newSubIngredient.Child_id &&
                (int)param.GetType().GetProperty("OrderNumber")!.GetValue(param)! == newSubIngredient.order_no)))
            .ReturnsAsync(1);

        // Act
        var result = await service.CreateSubIngredientAsync(newSubIngredient);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateSubIngredientAsync()
    {
        // Arrange
        var oldSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 3, order_no = 3 };
        var newSubIngredient = new SubIngredientDTO { Parent_id = 1, Child_id = 3, order_no = 4 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "UPDATE subingredient SET parent_id = @NewParentId, child_id = @NewChildId, order_no = @NewOrderNumber WHERE parent_id = @OldParentId AND child_id = @OldChildId",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("NewParentId")!.GetValue(param)! == newSubIngredient.Parent_id &&
                (int)param.GetType().GetProperty("NewChildId")!.GetValue(param)! == newSubIngredient.Child_id &&
                (int)param.GetType().GetProperty("NewOrderNumber")!.GetValue(param)! == newSubIngredient.order_no &&
                (int)param.GetType().GetProperty("OldParentId")!.GetValue(param)! == oldSubIngredient.Parent_id &&
                (int)param.GetType().GetProperty("OldChildId")!.GetValue(param)! == oldSubIngredient.Child_id)))
            .ReturnsAsync(1);

        // Act
        var result = await service.UpdateSubIngredientAsync(oldSubIngredient, newSubIngredient);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task DeleteSubIngredientAsync()
    {
        // Arrange
        var subIngredientToDelete = new SubIngredientDTO { Parent_id = 1, Child_id = 3, order_no = 4 };
        mockDapperExecutor.Setup(x => x.ExecuteAsync(
            "DELETE FROM subingredient WHERE parent_id = @ParentId AND child_id = @ChildId",
            It.Is<object>(param =>
                (int)param.GetType().GetProperty("ParentId")!.GetValue(param)! == subIngredientToDelete.Parent_id &&
                (int)param.GetType().GetProperty("ChildId")!.GetValue(param)! == subIngredientToDelete.Child_id)))
            .ReturnsAsync(1);

        // Act
        var result = await service.DeleteSubIngredientAsync(subIngredientToDelete);

        // Assert
        Assert.Equal(1, result);
    }
}